﻿using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Domain.Entities;
using DEKL.CP.Infra.Data.EF.Context;
using System.Linq;

namespace DEKL.CP.Infra.Data.EF.Repositories
{
    public class UsuarioRepositoryEF : RepositoryEF<Usuario>, IUsuarioRepository
    {
        public UsuarioRepositoryEF(DEKLCPDataContextEF ctx) : base(ctx)
        { }

        public Usuario Get(string email) 
            => _ctx.Usuarios.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }
}
