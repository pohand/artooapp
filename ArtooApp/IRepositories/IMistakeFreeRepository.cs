using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IMistakeFreeRepository
    {
        IEnumerable<MistakeFree> MistakeFree { get; }
        MistakeFree GetMistakeById(int mistakeFreeId);
        List<MistakeFree> GetMistakeFreeByInspectionId(int inspectionId);
        void InsertMistakeFree(MistakeFree mistakeFree);
    }
}
