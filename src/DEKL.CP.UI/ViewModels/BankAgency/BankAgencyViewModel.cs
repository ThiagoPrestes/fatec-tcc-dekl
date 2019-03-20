﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.UI.ViewModels.Address;
using DEKL.CP.UI.ViewModels.InternalBankAccount;
using DEKL.CP.UI.ViewModels.Provider;

namespace DEKL.CP.UI.ViewModels.BankAgency
{
    public class BankAgencyViewModel : IBankAgency
    {
        public int Id { get; set; }

        [Required, DisplayName("Número")]
        public short Number { get; set; }

        public int? AddressId { get; set; }

        public AddressViewModel Address { get; set; }

        [DisplayName("Nome do(a) gerente")]
        public string ManagerName { get; set; }

        [DisplayName("Telefone")]
        public string PhoneNumber { get; set; }

        [DisplayName("E-mail"), EmailAddress]
        public string Email { get; set; }

        [Required, DisplayName("Banco")]
        public int BankId { get; set; }

        public virtual Domain.Entities.Bank Bank { get; set; }

        public virtual ICollection<InternalBankAccountViewModel> InternalBankAccounts { get; set; }

        public virtual ICollection<ProviderBankAccountViewModel> ProviderBankAccounts { get; set; }

        [DisplayName("Agência")]
        public string BankAgencyDescription => $"{Number} - {Bank.Name}";
    }
}
