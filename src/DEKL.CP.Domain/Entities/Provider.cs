﻿using System.Collections.Generic;
using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.Domain.Enums;

namespace DEKL.CP.Domain.Entities
{
    public class Provider : EntityBase, IProvider
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public virtual Address Address { get; set; }
        public TypeProvider TypeProvider { get; set; }  
        public virtual ICollection<AccountToPay> AccountsToPay { get; set; }
        public virtual ICollection<ProviderBankAccount> ProviderBankAccounts { get; set; }
    }
}