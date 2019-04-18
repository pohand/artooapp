using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IFinalWeekRepository
    {
        int CreateFinalWeek(FinalWeek finalWeek);

        void UpdateFinalWeek(FinalWeek finalWeek);
        void DeleteFinalWeek(int finalWeekId);

        IEnumerable<FinalWeek> FinalWeeks { get; }

        FinalWeek GetFinalWeekById(int finalWeekId);
    }
}
