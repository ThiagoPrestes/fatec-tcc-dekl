﻿using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.Domain.Entities;
using DEKL.CP.Domain.Entities.Filters;
using System;
using System.Collections.Generic;

namespace DEKL.CP.Domain.Contracts.Repositories
{
    public interface IAccountToPayRepository : IRepository<AccountToPay>
    {
        IEnumerable<IAccountToPayRelashionships> AccountToPayActivesRelashionships { get; }
        IEnumerable<IAccountToPayRelashionships> AccountToPayReport(AccountsToPayFilter AccountToPayFilterData);
    }
}