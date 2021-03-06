﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.UI.ViewModels.AccountsToPay;

namespace DEKL.CP.UI.ViewModels.Provider
{
    public class ProviderLegalPersonViewModel : ProviderViewModel, IProviderLegalPerson
    {
        [Required(AllowEmptyStrings = false), DisplayFormat(ConvertEmptyStringToNull = false), DisplayName("Razão Social")]
        public string CorporateName { get; set; }

        [Required]
        public string CNPJ { get; set; }

        [DisplayName("Inscrição Municipal")]
        public string MunicipalRegistration { get; set; }

        [DisplayName("Inscrição Estadual")]
        public string StateRegistration { get; set; }

        public virtual ICollection<AccountToPayViewModel> AccountsToPay { get; set; }
    }
}
