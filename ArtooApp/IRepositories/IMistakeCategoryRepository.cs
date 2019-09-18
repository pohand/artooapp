using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IMistakeCategoryRepository
    {
        void CreateMistakeCategory(MistakeCategory mistake);
        void UpdateMistakeCategory(MistakeCategory mistake);
        void DeleteMistakeCategory(int mistakeCategoryId);
        IEnumerable<MistakeCategory> MistakeCategories { get; }
        MistakeCategory GetMistakeCategoryById(int mistakeCategoryId);
    }
}
