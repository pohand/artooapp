using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Artoo.IRepositories;
using Artoo.Helpers;

namespace Artoo.Repositories
{
    public class InspectionRepository : IInspectionRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ITechManagerRepository _techManagerRepository;
        public InspectionRepository(AppDbContext appDbContext, ITechManagerRepository techManagerRepository)
        {
            _appDbContext = appDbContext;
            _techManagerRepository = techManagerRepository;
        }
        public IEnumerable<Inspection> Inspections => _appDbContext.Inspections;

        public void BookInspection(int inspectionId, string userId)
        {
            var existing = _appDbContext.Inspections.SingleOrDefault(x => x.InspectionId == inspectionId);

            existing.BookingStatus = true;
            existing.UserBookingId = userId;

            _appDbContext.Inspections.Update(existing);
            _appDbContext.SaveChanges();
        }

        public void DeleteInspection(int inspectionId)
        {
            var inspection = _appDbContext.Inspections.SingleOrDefault(x => x.InspectionId == inspectionId);
            _appDbContext.Remove(inspection);
            _appDbContext.SaveChanges();
        }

        public Inspection GetInspectionById(int inspectionId)
        {
            return _appDbContext.Inspections.FirstOrDefault(p => p.InspectionId == inspectionId);
        }

        public void UpdateInspection(Inspection inspection)
        {
            var existing = _appDbContext.Inspections.SingleOrDefault(x => x.InspectionId == inspection.InspectionId);

            if (existing != null)
            {
                //existing.Description = inspection.Description;
                //existing.IMAN = inspection.IMAN;
                //existing.MistakeLines = inspection.MistakeLines;
                //existing.Model = inspection.Model;
                //existing.OrderNumber = inspection.OrderNumber;
                //existing.OrderQuantity = inspection.OrderQuantity;
                //existing.OrderType = inspection.OrderType;
                //existing.PassionBrand = inspection.PassionBrand;
                //existing.PONumber = inspection.PONumber;

                existing.IsThirdParty = inspection.IsThirdParty;
                existing.InspectDate = inspection.InspectDate;
                existing.InspectStatus = inspection.InspectStatus;
                existing.NumberChecked = inspection.NumberChecked;
                existing.Parameter = inspection.Parameter;
                existing.Result = inspection.Result;
                existing.Username = inspection.Username;
                existing.ProductQuantityChecked = inspection.ProductQuantityChecked;
                existing.CustomerComment = inspection.CustomerComment;
                existing.DateChecked = DateTime.Now;

                _appDbContext.Inspections.Update(existing);

                _appDbContext.SaveChanges();
            }
        }

        public void AddInspectionMistakeDetail(InspectionMistakeDetail imd)
        {
            _appDbContext.InspectionMistakeDetails.Add(imd);
            _appDbContext.SaveChanges();
        }

        public void ApproveInspection(Inspection inspection)
        {
            var existing = _appDbContext.Inspections.SingleOrDefault(x => x.InspectionId == inspection.InspectionId);
            if (existing != null)
            {
                existing.ApproveUsername = inspection.ApproveUsername;

                _appDbContext.Inspections.Update(existing);

                _appDbContext.SaveChanges();
            }
        }

        public void UnBookInspection(int inspectionId)
        {
            var existing = _appDbContext.Inspections.SingleOrDefault(x => x.InspectionId == inspectionId);

            existing.BookingStatus = false;
            existing.UserBookingId = null;

            _appDbContext.Inspections.Update(existing);
            _appDbContext.SaveChanges();
        }

        public IEnumerable<InspectionMistakeDetail> InspectionMistakeDetails => _appDbContext.InspectionMistakeDetails;

        public void DeleteInspectByOrderNumbers(List<string> orderNumbers)
        {
            foreach (var item in orderNumbers)
            {
                var existing = _appDbContext.Inspections.Where(x => x.OrderNumber == item && x.BookingStatus == false);
                if (existing.Count() > 0)
                {
                    var inspections = existing.Where(x => x.OrderNumber == item).ToList();
                    foreach (var inspection in inspections)
                    {
                        _appDbContext.Inspections.Remove(inspection);
                        _appDbContext.SaveChanges();
                    }
                    //_appDbContext.Inspections.Remove(inspection);
                    //_appDbContext.SaveChanges();
                }
            }
        }

        public void InsertList(List<Inspection> inspectionList)
        {
            foreach (var item in inspectionList)
            {
                var existing = _appDbContext.Inspections.Any(x => x.OrderNumber == item.OrderNumber);
                if (!existing)
                {
                    _appDbContext.Inspections.AddRange(item);
                }
            }
            _appDbContext.SaveChanges();
        }

        public void UpdateFactorty(List<Inspection> inspectionList)
        {
            var factories = _appDbContext.Factories.ToList();
            var inspections = _appDbContext.Inspections.ToList();
            foreach (var item in inspections)
            {

                var existingFactory = factories.FirstOrDefault(x => x.Name == item.FactoryName);
                //if finalweek is existing, get the existing FinalWeekID and assign it to inspection
                item.FactoryId = existingFactory.FactoryId;
                _appDbContext.Inspections.Update(item);
                _appDbContext.SaveChanges();
            }
            //foreach (var item in inspectionList)
            //{
            //    var existing = _appDbContext.Inspections.Any(x => x.OrderNumber == item.OrderNumber);
            //    if (existing)
            //    {
            //        var i = _appDbContext.Inspections.FirstOrDefault(x => x.OrderNumber == item.OrderNumber);
            //        i.FactoryName = item.FactoryName;
            //        i.FactoryId = item.FactoryId;
            //        _appDbContext.Inspections.Update(i);
            //        _appDbContext.SaveChanges();
            //    }
            //}
        }

        public void UpdateWeekFinal(List<Inspection> inspectionList)
        {
            var finalWeeks = _appDbContext.FinalWeeks.ToList();
            var inspections = _appDbContext.Inspections.ToList();
            var importHelper = new ImportHelper();
            foreach (var item in inspections)
            {
                int weekOfYear = 0;
                weekOfYear = importHelper.GetIso8601WeekOfYear(item.FinalDate);

                if (weekOfYear != 0)
                {
                    var existingFinalWeek = finalWeeks.FirstOrDefault(x => x.Name == item.FinalDate.Year.ToString() + "-W" + weekOfYear);
                    //if finalweek is existing, get the existing FinalWeekID and assign it to inspection
                    item.FinalWeekId = existingFinalWeek.FinalWeekId;
                    _appDbContext.Inspections.Update(item);
                    _appDbContext.SaveChanges();
                }
            }

            //foreach (var item in inspectionList)
            //{
            //    var existing = _appDbContext.Inspections.FirstOrDefault(x => x.OrderNumber.Contains(item.OrderNumber));
            //    if (existing != null)
            //    {
            //        existing.FinalWeekId = item.FinalWeekId;
            //        _appDbContext.Inspections.Update(existing);
            //        _appDbContext.SaveChanges();
            //    }
            //}
        }

        public void UpdateTechManager(List<Inspection> inspectionList)
        {
            try
            {
                var techManagers = _appDbContext.TechManagers.ToList();
                var inspections = _appDbContext.Inspections.ToList();
                foreach (var item in inspections)
                {

                    var existingTechManager = techManagers.FirstOrDefault(x => x.Name == item.TechManagerName);
                    //if finalweek is existing, get the existing FinalWeekID and assign it to inspection
                    if (existingTechManager == null)
                    {
                        var newTechManager = new TechManager()
                        {
                            Name = item.TechManagerName
                        };

                        item.TechManagerId = _techManagerRepository.CreateTechManager(newTechManager);
                    }
                    else
                    {
                        item.TechManagerId = existingTechManager.TechManagerId;
                    }
                    _appDbContext.Inspections.Update(item);
                    _appDbContext.SaveChanges();
                }

                //inspectionList = inspectionList.Where(x => x.FactoryName == "2").ToList();
                //foreach (var item in inspectionList)
                //{
                //    var existing = _appDbContext.Inspections.FirstOrDefault(x => x.OrderNumber.Contains(item.OrderNumber));
                //    if (existing != null)
                //    {
                //        existing.TechManagerName = item.TechManagerName;
                //        _appDbContext.Inspections.Update(existing);
                //        _appDbContext.SaveChanges();
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
