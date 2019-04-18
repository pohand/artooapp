using Artoo.Common;
using Artoo.Models;
using Artoo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.IRepositories;

namespace Artoo.Repositories
{
    public class MistakeRepository : IMistakeRepository
    {
        private readonly AppDbContext _appDbContext;

        public MistakeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Mistake> Mistakes => _appDbContext.Mistakes;


        public void CreateMistake(Mistake mistake)
        {
            var existingmistake = _appDbContext.Mistakes.FirstOrDefault(x => x.Name == mistake.Name);

            if (existingmistake == null)
            {

                mistake.DateRegister = DateTime.Now;

                _appDbContext.Mistakes.Add(mistake);

                _appDbContext.SaveChanges();
            }
        }

        public void DeleteMistake(int mistakeId)
        {
            var mistake = _appDbContext.Mistakes.SingleOrDefault(x => x.MistakeId == mistakeId);
            _appDbContext.Remove(mistake);
            _appDbContext.SaveChanges();
        }

        public Mistake GetMistakeById(int mistakeId)
        {
            return _appDbContext.Mistakes.FirstOrDefault(p => p.MistakeId == mistakeId);
        }

        public void UpdateMistake(Mistake mistake)
        {
            var existing = _appDbContext.Mistakes.FirstOrDefault(x => x.MistakeId == mistake.MistakeId);

            if(existing != null)
            {
                existing.Description = mistake.Description;
                existing.ManualType = mistake.ManualType;
                existing.Name = mistake.Name;
                existing.Status = mistake.Status;
                existing.ImageUrl = mistake.ImageUrl;

                _appDbContext.Mistakes.Update(existing);

                _appDbContext.SaveChanges();
            }
        }

        public List<Mistake> GetMistakeByInspectionId(int inspectionId)
        {
            var mistakeListId = _appDbContext.InspectionMistakeDetails.Where(p => p.InspectionId == inspectionId).Select(x=>x.MistakeId).ToList();

            List<Mistake> mistakeList = new List<Mistake>();

            foreach (var item in mistakeListId)
            {
                var mistake = _appDbContext.Mistakes.FirstOrDefault(x => x.MistakeId == item);

                mistakeList.Add(mistake);
            }

            return mistakeList;
        }

        public List<MistakeViewModel> GetMistakeDetailByInspectionId(int inspectionId)
        {
            var mistakeDetailList = _appDbContext.InspectionMistakeDetails.Where(p => p.InspectionId == inspectionId);

            List<MistakeViewModel> mistakeList = new List<MistakeViewModel>();

            foreach (var item in mistakeDetailList)
            {
                var mistake = _appDbContext.Mistakes.FirstOrDefault(x => x.MistakeId == item.MistakeId);

                if (mistake != null)
                {
                    var mistakeDetail = new MistakeViewModel()
                    {
                        Name = mistake.Name,
                        MistakeId = mistake.MistakeId,
                        Quantity = item.Quantity
                    };

                    mistakeList.Add(mistakeDetail);
                }
            }

            return mistakeList;
        }

        public List<Mistake> GetMistakeByPrefix(string prefix, MistakeEnum mistakeType)
        {
            if (mistakeType == MistakeEnum.ManualMistake)
            {
                return _appDbContext.Mistakes.Where(p => p.Name.Contains(prefix) && p.ManualType == (int)MistakeEnum.ManualMistake).ToList();
            }
            else
            {
                return _appDbContext.Mistakes.Where(p => p.Name.Contains(prefix) && p.ManualType == (int)MistakeEnum.DeviceMistake).ToList();
            }
        }

        public bool CheckExistingMistakeDetailById(int mistakeId)
        {
            return _appDbContext.InspectionMistakeDetails.Any(x => x.MistakeId == mistakeId);
        }
    }
}
