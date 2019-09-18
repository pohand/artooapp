using Artoo.Common;
using Artoo.IRepositories;
using Artoo.IServices;
using Artoo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Artoo.Services
{
    public class MistakeCategoryService : IMistakeCategoryService
    {
        private readonly IMistakeCategoryRepository _mistakeCategoryRepository;
        private readonly AppDbContext _appDbContext;
        public MistakeCategoryService(AppDbContext appDbContext,
            IMistakeCategoryRepository mistakeCategoryRepository)
        {
            _appDbContext = appDbContext;
            _mistakeCategoryRepository = mistakeCategoryRepository;
        }

        public List<SelectListItem> MistakeCategories()
        {
            return _mistakeCategoryRepository.MistakeCategories
               .Select(r => new SelectListItem
               {
                   Text = r.Name,
                   Value = r.MistakeCategoryID.ToString()
               }).ToList();
        }

        public List<SelectListItem> MistakeCategoriesByType(MistakeEnum mistakeType)
        {
            return _mistakeCategoryRepository.MistakeCategories.Where(x=>x.MistakeType == (int)mistakeType)
               .Select(r => new SelectListItem
               {
                   Text = r.Name,
                   Value = r.MistakeCategoryID.ToString()
               }).ToList();
        }
    }
}
