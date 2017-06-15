using SBS_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SBS_Ecommerce.Framework.Repositories
{
    public class SBSUnitWork : IDisposable
    {
        private SBS_Entities entities;
        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

        public SBSUnitWork()
        {
            entities = new SBS_Entities();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity:class
        {
            if (repositories.ContainsKey(typeof(TEntity)) == true)
            {
                return repositories[typeof(TEntity)] as SBSRepository<TEntity>;
            }
            IRepository<TEntity> repo = new SBSRepository<TEntity>(entities);
            repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public void SaveChanges()
        {
            entities.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await entities.SaveChangesAsync();
        }

        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool v)
        {
            if (!disposedValue)
            {
                if (v)
                {
                    entities.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}