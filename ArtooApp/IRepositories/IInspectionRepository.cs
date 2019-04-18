using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IInspectionRepository
    {
        void UpdateInspection(Inspection inspection);
        void DeleteInspection(int inspectionId);
        IEnumerable<Inspection> Inspections { get; }
        Inspection GetInspectionById(int inspectionId);
        void BookInspection(int inspectionId, string userId);
        void AddInspectionMistakeDetail(InspectionMistakeDetail imd);
        void ApproveInspection(Inspection inspection);
        void UnBookInspection(int inspectionId);
        IEnumerable<InspectionMistakeDetail> InspectionMistakeDetails { get; }
        void DeleteInspectByOrderNumbers(List<string> orderNumbers);
        void InsertList(List<Inspection> inspectionList);
        void UpdateFactorty(List<Inspection> inspectionList);
        void UpdateWeekFinal(List<Inspection> inspectionList);
        void UpdateTechManager(List<Inspection> inspectionList);
    }
}
