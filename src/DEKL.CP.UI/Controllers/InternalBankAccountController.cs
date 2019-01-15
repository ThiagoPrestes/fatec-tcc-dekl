﻿using AutoMapper;
using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Domain.Entities;
using DEKL.CP.UI.Scripts.Toastr;
using DEKL.CP.UI.ViewModels.InternalBankAccount;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace DEKL.CP.UI.Controllers
{
    public class InternalBankAccountController : Controller
    {
        private readonly IInternalBankAccountRepository _internalBankAccountRepository;

        public InternalBankAccountController(IInternalBankAccountRepository internalBankAccountRepository) 
            => _internalBankAccountRepository = internalBankAccountRepository;

        public ActionResult Index() 
            => View(Mapper.Map<IEnumerable<InternalBankAccountViewModel>>(_internalBankAccountRepository.Actives));

        public ActionResult Create()
        {
            ViewBag.BankAgencies = new SelectList(_internalBankAccountRepository.BankAgencyesActives, nameof(BankAgency.Id),
                nameof(BankAgency.BankAgencyDescription));

            return View();
        }

        [HttpPost]
        public ActionResult Create(InternalBankAccount internalBankAccount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _internalBankAccountRepository.Add(internalBankAccount);

                    this.AddToastMessage("Conta salva", $"A conta {internalBankAccount.Name} foi salva com sucesso", ToastType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro no salvamento", $"Erro ao salvar a conta {internalBankAccount.Name}, favor tentar novamente",
                        ToastType.Error);
                }
            }

            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var internalBankAccount = _internalBankAccountRepository.FindActive(id.Value);

            return View(Mapper.Map<InternalBankAccountViewModel>(internalBankAccount));
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var providerBankAccount = _internalBankAccountRepository.FindActive(id.Value);
            if (providerBankAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.BankAgencies = new SelectList(_internalBankAccountRepository.BankAgencyesActives, nameof(BankAgency.Id),
                nameof(BankAgency.BankAgencyDescription));
            return View(Mapper.Map<InternalBankAccountViewModel>(providerBankAccount));
        }

        [HttpPost, Authorize(Roles = "Administrador"), ValidateAntiForgeryToken]
        public ActionResult Edit(InternalBankAccountViewModel internalBankAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var internalBankAccount = _internalBankAccountRepository.FindActive(internalBankAccountViewModel.Id);

                    internalBankAccount.Number = internalBankAccountViewModel.Number;
                    internalBankAccount.Name = internalBankAccountViewModel.Name;
                    internalBankAccount.BankAgencyId = internalBankAccountViewModel.BankAgencyId;
                    internalBankAccount.Balance = internalBankAccountViewModel.Balance;

                    _internalBankAccountRepository.Update(internalBankAccount);

                    this.AddToastMessage("Conta Editada", $"a conta {internalBankAccount.Name} foi editada com sucesso",
                        ToastType.Success);

                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Edição", $"Erro ao editar a conta {internalBankAccountViewModel.Name}, " +
                        $"favor tentar novamente", ToastType.Error);
                }
            }

            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var internalBankAccount = _internalBankAccountRepository.Find(id.Value);

            return internalBankAccount == null ? 
                HttpNotFound() : 
                (ActionResult)View(Mapper.Map<InternalBankAccountViewModel>(internalBankAccount));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var internalBankAccount = _internalBankAccountRepository.Find(id.Value);

                try
                {
                    if (internalBankAccount == null)
                    {
                        return HttpNotFound();
                    }

                    _internalBankAccountRepository.DeleteLogical(internalBankAccount);

                    this.AddToastMessage("Conta excluída", $"A conta {internalBankAccount.Name} foi excluída com sucesso", ToastType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Exclusão", $"Erro ao excluir a conta {internalBankAccount?.Name}, favor tentar novamente",
                        ToastType.Error);
                }
            }

            return View();
        }
    }
}