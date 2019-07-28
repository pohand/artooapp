using Artoo.Common;
using Artoo.Models;
using Artoo.ViewModels;
using System.Collections.Generic;

namespace Artoo.IRepositories
{
    public interface IMistakeRepository
    {
        void CreateMistake(Mistake mistake);

        void UpdateMistake(Mistake mistake);
        void DeleteMistake(int mistakeId);

        IEnumerable<Mistake> Mistakes { get; }

        Mistake GetMistakeById(int mistakeId);
        List<Mistake> GetMistakeByInspectionId(int inspectionId);
        List<Mistake> GetMistakeByPrefix(string prefix, MistakeEnum mistakeType);
        List<Mistake> GetMistakeByPrefix(string prefix, MistakeEnum mistakeType, int mistakeCategoryId);
        List<MistakeViewModel> GetMistakeDetailByInspectionId(int inspectionId);
        bool CheckExistingMistakeDetailById(int mistakeId);
        List<Mistake> GetMistakesByCategory(int mistakeCategoryId);
    }
}
