using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.IRepositories;

namespace Artoo.Repositories
{
    public class FactoryRepository : IFactoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public FactoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<Factory> Factories => _appDbContext.Factories;

        public int CreateFactory(Factory factory)
        {
            factory.DateRegister = DateTime.Now;

            _appDbContext.Factories.Add(factory);

            _appDbContext.SaveChanges();

            return factory.FactoryId;
        }

        public void DeleteFactory(int factoryId)
        {
            var factories = _appDbContext.Factories.SingleOrDefault(x => x.FactoryId == factoryId);
            _appDbContext.Remove(factories);
            _appDbContext.SaveChanges();
        }

        public Factory GetFactoryById(int factoryId)
        {
            return _appDbContext.Factories.FirstOrDefault(p => p.FactoryId == factoryId);
        }

        public void UpdateFactory(Factory factory)
        {
            var existing = _appDbContext.Factories.SingleOrDefault(x => x.FactoryId == factory.FactoryId);

            if (existing != null)
            {
                existing.Description = factory.Description;
                existing.Name = factory.Name;
                existing.Status = factory.Status;

                _appDbContext.Factories.Update(existing);

                _appDbContext.SaveChanges();
            }
        }
    }
}
