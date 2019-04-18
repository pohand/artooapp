using Artoo.Common;
using Artoo.Models;
using Artoo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        List<MistakeViewModel> GetMistakeDetailByInspectionId(int inspectionId);
        bool CheckExistingMistakeDetailById(int mistakeId);
    }
}
