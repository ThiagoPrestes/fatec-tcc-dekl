﻿using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Domain.Entities;
using DEKL.CP.Domain.Enums;
using DEKL.CP.UI.Scripts.Toastr;
using DEKL.CP.UI.ViewModels.Provider;
using Microsoft.AspNet.Identity;

namespace DEKL.CP.UI.Controllers
{
    [Authorize]
    public class ProviderController : Controller
    {
        private readonly IProviderRepository _providerRepository;

        public ProviderController(IProviderRepository providerRepository) => _providerRepository = providerRepository;

        public ActionResult Index()
            => View(Mapper.Map<IEnumerable<ProviderPhysicalLegalPersonViewModel>>(_providerRepository.AllActivesProviderPhysicalLegalPerson));

        public ActionResult Select() => View();

        public ActionResult Create(TypeProvider typeprovider)
            => View(typeprovider == TypeProvider.PhysicalPerson ? "CreateProviderPhysicalPerson" : "CreateProviderLegalPerson");

        public ActionResult CreateProviderPhysicalPerson(ProviderPhysicalPerson providerPhysicalPerson)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    providerPhysicalPerson.ApplicationUserId = User.Identity.GetUserId<int>();

                    _providerRepository.Add(providerPhysicalPerson);

                    this.AddToastMessage("Fornecedor salvo", $"O fornecedor {providerPhysicalPerson.Name} foi salvo com sucesso", ToastType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro no salvamento", "Erro ao salvar o fornecedor, favor tentar novamente", ToastType.Error);
                    return View(Mapper.Map<ProviderPhysicalPersonViewModel>(providerPhysicalPerson));
                }
            }

            return View();
        }

        public ActionResult CreateProviderLegalPerson(ProviderLegalPerson providerLegalPerson)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    providerLegalPerson.ApplicationUserId = User.Identity.GetUserId<int>();

                    _providerRepository.Add(providerLegalPerson);

                    this.AddToastMessage("Banco salvo", $"O banco {providerLegalPerson.CorporateName} foi salvo com sucesso",
                        ToastType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro no salvamento", "Erro ao salvar o fornecedor, favor tentar novamente", ToastType.Error);
                    return View(Mapper.Map<ProviderLegalPersonViewModel>(providerLegalPerson));
                }
            }

            return View();
        }

        public ActionResult Details(int? id, TypeProvider typeProvider)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return typeProvider == TypeProvider.PhysicalPerson ?
                View("DetailsProviderPhysicalPerson", Mapper.Map<ProviderPhysicalPersonViewModel>(_providerRepository.FindActiveProviderPhysicalPerson(id.Value))) :
                View("DetailsProviderLegalPerson", Mapper.Map<ProviderLegalPersonViewModel>(_providerRepository.FindActiveProviderLegalPerson(id.Value)));
        }

        public ActionResult Edit(int? id, TypeProvider typeProvider)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (typeProvider == TypeProvider.PhysicalPerson)
            {
                var providerPhysicalPerson = _providerRepository.FindActiveProviderPhysicalPerson(id.Value);

                if (providerPhysicalPerson == null)
                {
                    return HttpNotFound();
                }

                return View("EditProviderPhysicalPerson", Mapper.Map<ProviderPhysicalPersonViewModel>(providerPhysicalPerson));
            }

            var providerLegalPerson = _providerRepository.FindActiveProviderLegalPerson(id.Value);

            if (providerLegalPerson == null)
            {
                return HttpNotFound();
            }

            return View("EditProviderLegalPerson", Mapper.Map<ProviderLegalPersonViewModel>(providerLegalPerson));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditProviderPhysicalPerson(ProviderPhysicalPersonViewModel providerPhysicalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    providerPhysicalPersonViewModel.ApplicationUserId = User.Identity.GetUserId<int>();

                    _providerRepository.Update(Mapper.Map<ProviderPhysicalPerson>(providerPhysicalPersonViewModel));

                    this.AddToastMessage("Fornecedor Editado", $"O fornecedor {providerPhysicalPersonViewModel.Name} foi editado com sucesso",
                        ToastType.Success);

                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Edição", $"Erro ao editar o Fornecedor {providerPhysicalPersonViewModel.Name}, " +
                        "favor tentar novamente", ToastType.Error);
                }
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditProviderLegalPerson(ProviderLegalPersonViewModel providerLegalPersonViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    providerLegalPersonViewModel.ApplicationUserId = User.Identity.GetUserId<int>();

                    _providerRepository.Update(Mapper.Map<ProviderLegalPerson>(providerLegalPersonViewModel));

                    this.AddToastMessage("Banco Editado", $"O fornecedor {providerLegalPersonViewModel.CorporateName} foi editado com sucesso",
                        ToastType.Success);

                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Edição", $"Erro ao editar o Fornecedor {providerLegalPersonViewModel.CorporateName}, " +
                        "favor tentar novamente", ToastType.Error);
                }
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

            var provider = _providerRepository.ActiveProviderPhysicalLegalPerson(id.Value);

            return provider == null ? HttpNotFound() : (ActionResult)View(Mapper.Map<ProviderPhysicalLegalPersonViewModel>(provider));
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

                var provider = _providerRepository.Find(id.Value);
                var nameProvider = (provider.TypeProvider == TypeProvider.PhysicalPerson) ?
                        _providerRepository.FindActiveProviderPhysicalPerson(id.Value).Name :
                        _providerRepository.FindActiveProviderLegalPerson(id.Value).CorporateName;

                try
                {
                    _providerRepository.DeleteLogical(provider);

                    this.AddToastMessage("Fornecedor excluído", $"O fornecedor {nameProvider} foi excluído com sucesso", ToastType.Success);
                    return RedirectToAction("Index");
                }
                catch
                {
                    this.AddToastMessage("Erro na Exclusão", $"Erro ao excluir o fornecedor {nameProvider}, favor tentar novamente",
                        ToastType.Error);
                }
            }

            return View();
        }
    }
}