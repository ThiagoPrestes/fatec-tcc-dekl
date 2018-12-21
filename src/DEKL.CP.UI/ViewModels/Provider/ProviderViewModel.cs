﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.Domain.Enums;

namespace DEKL.CP.UI.ViewModels.Provider
{
    public class ProviderViewModel : IProvider
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Telefone")]
        public string PhoneNumber { get; set; }

        [EmailAddress, DisplayName("E-mail")]
        public string Email { get; set; }

        public AddressViewModel Address { get; set; }

        public TypeProvider TypeProvider { get; set; }
    }
}