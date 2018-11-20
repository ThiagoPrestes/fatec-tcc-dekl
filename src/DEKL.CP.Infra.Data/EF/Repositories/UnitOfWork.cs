﻿using DEKL.CP.Infra.Data.EF.Context;
using System;
using System.Collections.Generic;
using DEKL.CP.Domain.Entities;

namespace DEKL.CP.Infra.Data.EF.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly DEKLCPDataContextEF _ctx;
        private bool _disposed;
        private Dictionary<string, object> _repositories;

        public UnitOfWork(DEKLCPDataContextEF ctx) => _ctx = ctx;

        public UnitOfWork() => _ctx = new DEKLCPDataContextEF();

        public void Save()
        {
            _ctx.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _ctx.Dispose();
            }

            _disposed = true;
        }

        public RepositoryEF<T> RepositoryEF<T>() where T : EntityBase
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }

            var type = typeof(T).Name;

            if (_repositories.ContainsKey(type))
                return  _repositories[type] as RepositoryEF<T>;

            var repositoryType = typeof(RepositoryEF<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _ctx);
            _repositories.Add(type, repositoryInstance);

            return _repositories[type] as RepositoryEF<T>;
        }
    }
}
