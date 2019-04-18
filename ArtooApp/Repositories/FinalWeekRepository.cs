using Artoo.IRepositories;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Repositories
{
    public class FinalWeekRepository : IFinalWeekRepository
    {
        private readonly AppDbContext _appDbContext;
        public FinalWeekRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<FinalWeek> FinalWeeks => _appDbContext.FinalWeeks;

        public int CreateFinalWeek(FinalWeek finalWeek)
        {
            finalWeek.DateRegister = DateTime.Now;

            _appDbContext.FinalWeeks.Add(finalWeek);

            _appDbContext.SaveChanges();

            return finalWeek.FinalWeekId;
        }

        public void DeleteFinalWeek(int finalWeekId)
        {
            var existing = _appDbContext.FinalWeeks.SingleOrDefault(x => x.FinalWeekId == finalWeekId);
            _appDbContext.Remove(existing);
            _appDbContext.SaveChanges();
        }

        public FinalWeek GetFinalWeekById(int finalWeekId)
        {
            return _appDbContext.FinalWeeks.FirstOrDefault(p => p.FinalWeekId == finalWeekId);
        }

        public void UpdateFinalWeek(FinalWeek finalWeek)
        {
            var existing = _appDbContext.FinalWeeks.SingleOrDefault(x => x.FinalWeekId == finalWeek.FinalWeekId);

            if (existing != null)
            {
                existing.Description = finalWeek.Description;
                existing.FinalWeekDay = finalWeek.FinalWeekDay;
                existing.Week = finalWeek.Week;
                existing.Year = finalWeek.Year;

                _appDbContext.FinalWeeks.Update(existing);

                _appDbContext.SaveChanges();
            }
        }
    }
}
