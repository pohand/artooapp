using Artoo.IRepositories;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Repositories
{
    public class MistakeCategoryRepository : IMistakeCategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public MistakeCategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<MistakeCategory> MistakeCategories => _appDbContext.MistakeCategories;

        public void CreateMistakeCategory(MistakeCategory mistakeCategory)
        {
            var existingMistakeCategory = _appDbContext.MistakeCategories.FirstOrDefault(x => x.Name == mistakeCategory.Name);

            if (existingMistakeCategory == null)
            {

                mistakeCategory.DateRegister = DateTime.Now;

                _appDbContext.MistakeCategories.Add(mistakeCategory);

                _appDbContext.SaveChanges();
            }
        }

        public void DeleteMistakeCategory(int mistakeCategoryId)
        {
            var mistakeCategory = _appDbContext.MistakeCategories.SingleOrDefault(x => x.MistakeCategoryID == mistakeCategoryId);
            _appDbContext.Remove(mistakeCategory);
            _appDbContext.SaveChanges();
        }

        public MistakeCategory GetMistakeCategoryById(int mistakeCategoryId) => _appDbContext.MistakeCategories.FirstOrDefault(p => p.MistakeCategoryID == mistakeCategoryId);

        public void UpdateMistakeCategory(MistakeCategory mistakeCategory)
        {
            var existing = _appDbContext.MistakeCategories.FirstOrDefault(x => x.MistakeCategoryID == mistakeCategory.MistakeCategoryID);

            if (existing != null)
            {
                existing.Description = mistakeCategory.Description;
                existing.Name = mistakeCategory.Name;
                existing.MistakeType = mistakeCategory.MistakeType;
                _appDbContext.MistakeCategories.Update(existing);

                _appDbContext.SaveChanges();
            }
        }
    }
}
