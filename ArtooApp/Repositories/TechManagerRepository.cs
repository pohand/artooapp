using Artoo.IRepositories;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Repositories
{
    public class TechManagerRepository : ITechManagerRepository
    {
        private readonly AppDbContext _appDbContext;
        public TechManagerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<TechManager> TechManagers => _appDbContext.TechManagers;

        public int CreateTechManager(TechManager techManager)
        {
            techManager.DateRegister = DateTime.Now;

            _appDbContext.TechManagers.Add(techManager);

            _appDbContext.SaveChanges();

            return techManager.TechManagerId;
        }

        public void DeleteTechManager(int techManagerId)
        {
            var techManager = _appDbContext.TechManagers.SingleOrDefault(x => x.TechManagerId == techManagerId);
            _appDbContext.Remove(techManager);
            _appDbContext.SaveChanges();
        }

        public TechManager GetTechManagerById(int techManagerId)
        {
            return _appDbContext.TechManagers.FirstOrDefault(p => p.TechManagerId == techManagerId);
        }

        public void UpdateTechManager(TechManager techManager)
        {
            var existing = _appDbContext.TechManagers.SingleOrDefault(x => x.TechManagerId == techManager.TechManagerId);

            if (existing != null)
            {
                existing.Description = techManager.Description;
                existing.Name = techManager.Name;
                existing.Status = techManager.Status;

                _appDbContext.TechManagers.Update(existing);

                _appDbContext.SaveChanges();
            }
        }
    }
}
