using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.IRepositories;

namespace Artoo.Repositories
{
    public class PassionBrandRepository : IPassionBrandRepository
    {
        private readonly AppDbContext _appDbContext;
        public PassionBrandRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<PassionBrand> PassionBrands => _appDbContext.PassionBrands;

        public int CreatePassionBrand(PassionBrand passionBrand)
        {
            passionBrand.DateRegister = DateTime.Now;

            _appDbContext.PassionBrands.Add(passionBrand);

            _appDbContext.SaveChanges();
            return passionBrand.PassionBrandId;
        }

        public void DeletePassionBrand(int passionBrandId)
        {
            var passionBrand = _appDbContext.PassionBrands.SingleOrDefault(x => x.PassionBrandId == passionBrandId);
            _appDbContext.Remove(passionBrand);
            _appDbContext.SaveChanges();
        }

        public PassionBrand GetPassionBrandById(int passionBrandId)
        {
            return _appDbContext.PassionBrands.FirstOrDefault(p => p.PassionBrandId == passionBrandId);
        }

        public void UpdatePassionBrand(PassionBrand passionBrand)
        {
            var existing = _appDbContext.PassionBrands.SingleOrDefault(x => x.PassionBrandId == passionBrand.PassionBrandId);

            if (existing != null)
            {
                existing.Description = passionBrand.Description;
                existing.Name = passionBrand.Name;
                existing.Status = passionBrand.Status;

                _appDbContext.PassionBrands.Update(existing);

                _appDbContext.SaveChanges();
            }
        }

        public int GetMaxPassionBrandId()
        {
            return _appDbContext.PassionBrands.OrderByDescending(u => u.PassionBrandId).Select(x => x.PassionBrandId).FirstOrDefault();
        }
    }
}
