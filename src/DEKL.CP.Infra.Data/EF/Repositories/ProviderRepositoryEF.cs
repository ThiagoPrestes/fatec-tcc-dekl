﻿using DEKL.CP.Domain.Contracts.Entities;
using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Infra.Data.DTO;
using DEKL.CP.Infra.Data.EF.Context;
using System.Collections.Generic;
using System.Linq;
using DEKL.CP.Domain.Entities;
using DEKL.CP.Domain.Enums;

namespace DEKL.CP.Infra.Data.EF.Repositories
{
    public class ProviderRepositoryEF : RepositoryEF<Provider>, IProviderRepository
    {
        private readonly DEKLCPDataContextEF _ctx;

        public ProviderRepositoryEF(DEKLCPDataContextEF ctx) : base(ctx) => _ctx = ctx;

        public IEnumerable<IProviderPhysicalLegalPerson> AllActivesProviderPhysicalLegalPerson
            => (
                    from p in _ctx.Providers
                    join ppp in _ctx.ProviderPhysicalPersons on p.Id equals ppp.Id
                    where p.Active
                    select new ProviderPhysicalLegalPersonDTO
                    {
                        Id = p.Id,
                        PhoneNumber = p.PhoneNumber,
                        Email = p.Email,
                        NameCorporateName = ppp.Name,
                        CPFCNPJ = ppp.CPF,
                        TypeProvider = TypeProvider.PhysicalPerson
                    }
                ).Union(
                        from p in _ctx.Providers
                        join plp in _ctx.ProviderLegalPersons on p.Id equals plp.Id
                        where p.Active
                        select new ProviderPhysicalLegalPersonDTO
                        {
                            Id = p.Id,
                            PhoneNumber = p.PhoneNumber,
                            Email = p.Email,
                            NameCorporateName = plp.CorporateName,
                            CPFCNPJ = plp.CNPJ,
                            TypeProvider = TypeProvider.LegalPerson
                        }
                ).AsEnumerable();

        public IProviderPhysicalLegalPerson ActiveProviderPhysicalLegalPerson (int id)
            => (
                    from p in _ctx.Providers
                    join ppp in _ctx.ProviderPhysicalPersons on p.Id equals ppp.Id
                    where p.Active && p.Id == id
                    select new ProviderPhysicalLegalPersonDTO
                    {
                        Id = p.Id,
                        PhoneNumber = p.PhoneNumber,
                        Email = p.Email,
                        NameCorporateName = ppp.Name,
                        CPFCNPJ = ppp.CPF,
                        TypeProvider = TypeProvider.PhysicalPerson
                    }
                ).Union(
                        from p in _ctx.Providers
                        join plp in _ctx.ProviderLegalPersons on p.Id equals plp.Id
                        where p.Active
                        select new ProviderPhysicalLegalPersonDTO
                        {
                            Id = p.Id,
                            PhoneNumber = p.PhoneNumber,
                            Email = p.Email,
                            NameCorporateName = plp.CorporateName,
                            CPFCNPJ = plp.CNPJ,
                            TypeProvider = TypeProvider.LegalPerson
                        }
                ).First();

        public ProviderPhysicalPerson FindActiveProviderPhysicalPerson (int id) => _ctx.ProviderPhysicalPersons.First(p => p.Active && p.Id == id);
        public ProviderLegalPerson FindActiveProviderLegalPerson(int id) => _ctx.ProviderLegalPersons.First(p => p.Active && p.Id == id);
    }
}