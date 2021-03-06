﻿using AutoMapper;
using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Domain.Entities;
using DEKL.CP.UI.Scripts.Toastr;
using DEKL.CP.UI.ViewModels.AccountsToPay;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DEKL.CP.Domain.Enums;
using DEKL.CP.UI.ViewModels.InternalBankAccount;
using DEKL.CP.UI.ViewModels.Provider;

namespace DEKL.CP.UI.Controllers
{
    [Authorize]
    public class AccountToPayController : Controller
    {
        private readonly IProviderRepository _providerRepository;
        private readonly IAccountToPayRepository _accountToPayRepository;
        private readonly IInternalBankAccountRepository _internalBankAccountRepository;
        private readonly IProviderBankAccountRepository _providerBankAccountRepository;

        public AccountToPayController(IAccountToPayRepository accountToPayRepository,
                                      IProviderRepository providerRepository,
                                      IInternalBankAccountRepository internalBankAccountRepository,
                                      IProviderBankAccountRepository providerBankAccountRepository)
        {
            _providerRepository = providerRepository;
            _internalBankAccountRepository = internalBankAccountRepository;
            _providerBankAccountRepository = providerBankAccountRepository;
            _accountToPayRepository = accountToPayRepository;
        }

        public ActionResult Index()
            => View(Mapper.Map<IEnumerable<AccountToPayRelashionships>>(_accountToPayRepository.AccountToPayActivesRelashionships));

        public ActionResult Create()
        {
            ViewBag.Providers = new SelectList(_providerRepository.AllActivesProviderPhysicalLegalPerson, 
                                               nameof(Provider.Id),
                                               nameof(IProviderPhysicalLegalPerson.NameCorporateName)
                                );
            return View();
        }

        [HttpPost]
        public ActionResult Create(AccountToPay accountToPay)
        {
            if (ModelState.IsValid)
            {
                try
                {  
                    // se é conta mensal não possuí parcelas
                    if (accountToPay.MonthlyAccount)
                    {
                        accountToPay.NumberOfInstallments = 0;
                    }

                    var valueInstallment = accountToPay.NumberOfInstallments == 0 ?
                        accountToPay.Value : accountToPay.Value / accountToPay.NumberOfInstallments;

                    // criação das parcelas                   
                    for (var i = 0; i < accountToPay.NumberOfInstallments; i++)
                    {
                        var installment = new Installment
                        {
                            MaturityDate = accountToPay.MaturityDate.AddMonths(i),
                            Value = valueInstallment,
                        };

                        accountToPay.Installments.Add(installment);
                    }

                    accountToPay.ApplicationUserId = User.Identity.GetUserId<int>();

                    _accountToPayRepository.Add(accountToPay);

                    this.AddToastMessage("Conta salva", $"A conta {accountToPay.Description} foi salva com sucesso", ToastType.Success);

                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro no salvamento", $"Erro ao salvar a conta {accountToPay.Description}, favor tentar novamente", ToastType.Error);

                }
            }

            return Create();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(Mapper.Map<AccountToPayViewModel>(_accountToPayRepository.Find(id.Value)));
        }

        public ActionResult Payment(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(Mapper.Map<AccountToPayViewModel>(_accountToPayRepository.Find(id.Value)));
        }

        public ActionResult InstallmentPayment(int? id, int installment_Id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var accountToPay = _accountToPayRepository.Find((int)id);

            ViewBag.Installment_Id = installment_Id;

            ViewBag.InternalBankAccounts = new SelectList(Mapper.Map<IEnumerable<InternalBankAccountRelashionshipsViewModel>>(_internalBankAccountRepository.InternalBankAccountRelashionships),
                                                          nameof(InternalBankAccountRelashionshipsViewModel.Id),
                                                          nameof(InternalBankAccountRelashionshipsViewModel.DescriptionAccount));

            ViewBag.ProviderBankAccounts = new SelectList(Mapper.Map<IEnumerable<ProviderBankAccountRelashionshipsViewModel>>(_providerBankAccountRepository.ProviderBankAccountRelashionships(accountToPay.ProviderId)),
                                                          nameof(ProviderBankAccountRelashionshipsViewModel.Id),
                                                          nameof(ProviderBankAccountRelashionshipsViewModel.DescriptionAccount));

            return View(Mapper.Map<AccountToPayViewModel>(_accountToPayRepository.Find(id.Value)));
        }

        public ActionResult PayAccount(int id, int installment_id, PaymentType paymentType, int? internalBankAccount_id,
                                      int? providerBankAccount_id)
        {
            if (id == 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var accountToPay = _accountToPayRepository.FindActive(id);

            try
            {
                var amountDue = 0M;

                //não possuí parcelas
                if (!accountToPay.Installments.Any())
                {
                    //conta em atraso
                    if (accountToPay.MaturityDate < DateTime.Now && accountToPay.PaidValue < accountToPay.Value)
                    {
                        var daysPastDue = (int)DateTime.Now.Subtract(accountToPay.MaturityDate).TotalDays;

                        amountDue = Math.Round(accountToPay.Value - accountToPay.Installments
                                                                     .Where(i => i.PaidValue >= i.Value)
                                                                     .Sum(i => i.Value), 2);

                        amountDue += Math.Round(amountDue * accountToPay.Penalty / 100, 2);
                        amountDue += Math.Round(amountDue * daysPastDue * accountToPay.DailyInterest / 100, 2);
                    }
                    //conta em dia
                    else
                    {
                        accountToPay.PaidValue = accountToPay.Value;
                    }

                    if (accountToPay.MonthlyAccount)
                    {
                        var newAccountToPay = new AccountToPay
                        {
                            MaturityDate = accountToPay.MaturityDate.AddMonths(1),
                            ApplicationUserId = User.Identity.GetUserId<int>(),
                            DailyInterest = accountToPay.DailyInterest,
                            Description = accountToPay.Description,
                            DocumentNumber = accountToPay.DocumentNumber,
                            MonthlyAccount = accountToPay.MonthlyAccount,
                            NumberOfInstallments = accountToPay.NumberOfInstallments,
                            PaymentSimulators = accountToPay.PaymentSimulators,
                            Penalty = accountToPay.Penalty,
                            Priority = accountToPay.Priority,
                            ProviderId = accountToPay.ProviderId,
                            Value = accountToPay.Value
                        };

                        _accountToPayRepository.Add(newAccountToPay);
                    }
                }
                //possuí parcelas
                else
                {
                    //vai pagar a conta inteira
                    if (installment_id == 0)
                    {
                        //parcelas vencidas
                        var overdueInstallments = accountToPay.Installments
                                                              .Where(i => i.MaturityDate < DateTime.Now && !i.PaymentDate.HasValue)
                                                              .ToList();

                        //parcelas em dia e ainda não pagas
                        var installmentsOk = accountToPay.Installments
                                                         .Except(overdueInstallments)
                                                         .Where(i => !i.PaymentDate.HasValue)
                                                         .ToList();

                        //valor total pago sem juros e mora
                        var withoutDue = Math.Round(amountDue = accountToPay.Value - accountToPay.Installments
                                                                                      .Except(overdueInstallments)
                                                                                      .Except(installmentsOk)
                                                                                      .Sum(i => i.Value), 2);

                        overdueInstallments.ForEach(o =>
                        {
                            var daysPastDue = (int)DateTime.Now.Subtract(o.MaturityDate).TotalDays;

                            amountDue += Math.Round(amountDue * accountToPay.Penalty / 100, 2);
                            amountDue += Math.Round(amountDue * daysPastDue * accountToPay.DailyInterest / 100, 2);
                        });

                        //parcelas em atraso
                        overdueInstallments.ForEach(i =>
                        {
                            i.PaymentDate = DateTime.Now;
                            i.PaidValue = Math.Round((amountDue - withoutDue) / overdueInstallments.Count, 2);
                        });

                        installmentsOk.ForEach(i =>
                        {
                            i.PaymentDate = DateTime.Now;
                            i.PaidValue = Math.Round(i.Value, 2);
                        });

                        amountDue += Math.Round(installmentsOk.Sum(i => i.Value), 2);
                    }

                    else
                    {
                        var installment = accountToPay.Installments.FirstOrDefault(i => i.Id == installment_id) ?? new Installment();

                        var daysPastDue = (int)DateTime.Now.Subtract(installment.MaturityDate).TotalDays;

                        if (daysPastDue > 0)
                        {
                            amountDue += installment.Value * accountToPay.Penalty / 100;
                            amountDue += amountDue * daysPastDue * accountToPay.DailyInterest / 100;
                        }
                        else
                        {
                            amountDue = installment.Value;
                        }
                    }
                }

                var internalBankAccount = paymentType != PaymentType.BankTransfer ?
                                          _internalBankAccountRepository.InternalBankAccountCaixa :
                                          _internalBankAccountRepository.FindActive(internalBankAccount_id ?? 0);

                accountToPay.PaymentType = paymentType;
                accountToPay.PaymentDate = DateTime.Now;
                accountToPay.PaidValue = amountDue;
                accountToPay.PaymentDate = accountToPay.Installments.All(i => i.PaymentDate.HasValue) ? DateTime.Now : (DateTime?)null;
                internalBankAccount.Balance -= amountDue;

                _accountToPayRepository.Update(accountToPay);
                _internalBankAccountRepository.Update(internalBankAccount);

                this.AddToastMessage("Conta Paga", $"A Conta {accountToPay.Description} foi paga com sucesso", ToastType.Success);
            }
            catch
            {
                this.AddToastMessage("Conta Paga", $"A Conta {accountToPay.Description} foi paga com sucesso", ToastType.Success);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var accountToPay = _accountToPayRepository.FindActive(id.Value);
            if (accountToPay == null)
            {
                return HttpNotFound();
            }

            ViewBag.Providers = new SelectList(_providerRepository.AllActivesProviderPhysicalLegalPerson, nameof(Provider.Id),
                nameof(IProviderPhysicalLegalPerson.NameCorporateName));

            return View(Mapper.Map<AccountToPayViewModel>(accountToPay));
        }

        [HttpPost, Authorize(Roles = "Administrador"), ValidateAntiForgeryToken]
        public ActionResult Edit(AccountToPayViewModel accountToPayViewModel)
        {
            try
            {
                var accountToPay = _accountToPayRepository.FindActive(accountToPayViewModel.Id);

                accountToPay.ProviderId = accountToPayViewModel.ProviderId != 0 ? accountToPayViewModel.ProviderId : accountToPay.ProviderId;
                accountToPay.MaturityDate = accountToPayViewModel.MaturityDate;
                accountToPay.DailyInterest = accountToPayViewModel.DailyInterest;
                accountToPay.Penalty = accountToPayViewModel.Penalty;
                accountToPay.Priority = accountToPayViewModel.Priority;
                accountToPay.Description = accountToPayViewModel.Description;
                accountToPay.Value = accountToPayViewModel.Value != 0 ? accountToPayViewModel.Value : accountToPay.Value;
                accountToPay.MonthlyAccount = accountToPayViewModel.MonthlyAccount;

                accountToPayViewModel.ApplicationUserId = User.Identity.GetUserId<int>();

                _accountToPayRepository.Update(accountToPay);

                this.AddToastMessage("Conta Editada", $"A Conta {accountToPayViewModel.Description} foi editada com sucesso", ToastType.Success);

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Providers = new SelectList(_providerRepository.AllActivesProviderPhysicalLegalPerson, 
                                                   nameof(Provider.Id),
                                                   nameof(IProviderPhysicalLegalPerson.NameCorporateName)
                                   );

                this.AddToastMessage("Erro na Edição", $"Erro ao editar a conta {accountToPayViewModel.Description}, favor tentar novamente", ToastType.Error);
            }

            return View();
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var accountToPay = _accountToPayRepository.Find(id.Value);

            return accountToPay == null ? HttpNotFound() : (ActionResult)View(Mapper.Map<AccountToPayViewModel>(accountToPay));
        }

        [HttpPost, Authorize(Roles = "Administrador"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var accountToPay = _accountToPayRepository.Find(id.Value);

                try
                {
                    if (accountToPay == null)
                    {
                        return HttpNotFound();
                    }

                    _accountToPayRepository.DeleteLogical(accountToPay);

                    this.AddToastMessage("Conta excluída", $"A Conta {accountToPay.Description} foi excluída com sucesso", ToastType.Success);

                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Exclusão", $"Erro ao excluir a Conta {accountToPay?.Description}, favor tentar novamente", ToastType.Error);
                }
            }

            return View();
        }
    }
}
