﻿using DEKL.CP.Domain.Contracts.Repositories;
using DEKL.CP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEKL.CP.Infra.Data.EF.Repositories
{
    public class RepositoryEF<T> : IRepository<T> where T : EntityBase
    {
        protected readonly DEKLCPDataContextEF _ctx;

        public RepositoryEF(DEKLCPDataContextEF ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<T> Get() => _ctx.Set<T>().ToList();

        public T Get(int id) => _ctx.Set<T>().Find(id);

        public void Add(T entity)
        {
            entity.DataCadastro = DateTime.Now;
            _ctx.Set<T>().Add(entity);
            Save();
        }

        public void Edit(T entity)
        {
            entity.DataAlteracao = DateTime.Now;
            _ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            Save();
        }

        public void Delete(T entity)
        {
            _ctx.Set<T>().Remove(entity);
            Save();
        }

        private void Save()
        {
            _ctx.SaveChanges();
        }

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}