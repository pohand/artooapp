using Artoo.IRepositories;
using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Artoo.Repositories
{
    public class MistakeFreeRepository : IMistakeFreeRepository
    {
        private readonly AppDbContext _appDbContext;

        public MistakeFreeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<MistakeFree> MistakeFree => _appDbContext.MistakeFrees;

        public MistakeFree GetMistakeById(int mistakeFreeId)
        {
            return _appDbContext.MistakeFrees.FirstOrDefault(p => p.MistakeFreeId == mistakeFreeId);
        }

        public List<MistakeFree> GetMistakeFreeByInspectionId(int inspectionId)
        {
            return _appDbContext.MistakeFrees
                .Where(p => p.InspectionId == inspectionId).ToList();
        }

        public void InsertMistakeFree(MistakeFree mistakeFree)
        {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (mistakeFree.InspectionId != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            {
                mistakeFree.DateRegister = DateTime.Now;
                _appDbContext.MistakeFrees.Add(mistakeFree);
                _appDbContext.SaveChanges();
            }
        }
    }
}
