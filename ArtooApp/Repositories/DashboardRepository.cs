using Artoo.Common;
using Artoo.Models;
using Artoo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Artoo.IRepositories;
using Artoo.Helpers;
using System.IO;

namespace Artoo.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _appDbContext;
        public static IConfiguration Configuration { get; set; }

        public DashboardRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public BlokingRateListViewModel GetBlockingRateByFactory()
        {

            DayOfWeek weekStart = DayOfWeek.Sunday; // or Sunday, or whenever
            DateTime startingDate = DateTime.Today;

            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            //lấy ngày thứ 2 cách đó 5 tuần
            DateTime previousFiveWeekStart = startingDate.AddDays(-35);

            //tổng số PO được inspect rồi
            var list = _appDbContext.Inspections.Where(t => t.InspectDate.Date >= previousFiveWeekStart.Date && t.InspectDate.Date < startingDate.Date && t.InspectStatus == true).ToList();

            var manufacturerList = (from so in _appDbContext.Inspections
                                    group so by so.FactoryName into TotaledFactories
                                    select new
                                    {
                                        FactoryName = TotaledFactories.Key,
                                    });
            List<BlokingRateViewModel> brList = new List<BlokingRateViewModel>();
            foreach (var item in manufacturerList)
            {
                var rejectPO = list.Where(x => x.Result == (int)InspectionResultEnum.Reject && x.FactoryName == item.FactoryName).Count();
                var totalPO = list.Where(x => x.FactoryName == item.FactoryName).Count();

                //var factoryId = list.FirstOrDefault(x => x.FactoryName == item.FactoryName).FactoryId;
                var blockingRate = totalPO == 0 ? 0 : (int)Math.Round((double)(100 * rejectPO) / totalPO);
                BlokingRateViewModel br = new BlokingRateViewModel()
                {
                    RejectPO = rejectPO,
                    TotalPO = totalPO,
                    FactoryName = item.FactoryName,
                    BlockingRate = blockingRate
                    //FactoryId = factoryId
                };

                brList.Add(br);
            }

            BlokingRateListViewModel result = new BlokingRateListViewModel()
            {
                BlockingRateList = brList,
                StartDate = previousFiveWeekStart.AddDays(1).ToString("d"),
                EndDate = startingDate.AddDays(-1).ToString("d")
            };

            return result;
        }

        public BlokingRateViewModel GetBlockingRate()
        {

            DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
            DateTime startingDate = DateTime.Today;

            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            DateTime previousFiveWeekStart = startingDate.AddDays(-35);

            var list = _appDbContext.Inspections.Where(t => t.InspectDate.Date >= previousFiveWeekStart.Date && t.InspectDate.Date < startingDate.Date && t.InspectStatus == true);

            var rejectPO = list.Where(x => x.Result == (int)InspectionResultEnum.Reject).Count();
            var totalPO = list.Count();
            var blockingRate = totalPO == 0 ? 0 : (int)Math.Round((double)(100 * rejectPO) / totalPO);

            BlokingRateViewModel result = new BlokingRateViewModel()
            {
                RejectPO = rejectPO,
                TotalPO = totalPO,
                BlockingRate = blockingRate
            };

            return result;
        }

        public List<NumberOfPONeedCheckViewModel> GetNumberOfPONeedToCheckListByFactory(BlokingRateListViewModel blockingRateList)
        {
            List<NumberOfPONeedCheckViewModel> numberOfPONeedToCheckList = new List<NumberOfPONeedCheckViewModel>();
            var importHelper = new ImportHelper();
            int weekOfYear = importHelper.GetIso8601WeekOfYear(DateTime.Now);
            int currentYear = DateTime.Now.Year;
            int finalWeekId = 0;
            var currentFinalWeek = _appDbContext.FinalWeeks.FirstOrDefault(x => x.Week == weekOfYear && x.Year == currentYear);
            if (currentFinalWeek != null)
            {
                finalWeekId = currentFinalWeek.FinalWeekId;
            }

            foreach (var item in blockingRateList.BlockingRateList)
            {
                NumberOfPONeedCheckViewModel numberOfPONeedToCheck = new NumberOfPONeedCheckViewModel();

                var blockingRate = item.TotalPO == 0 ? 0 : (int)Math.Round((double)(100 * item.RejectPO) / item.TotalPO);

                var numberRate = (blockingRate - 1) / 5;

                var totalPO = _appDbContext.Inspections.Where(t => t.FinalWeekId == finalWeekId && t.FactoryName == item.FactoryName).Count();
                var listOfImplantation = _appDbContext.Inspections.Where(t => t.FinalWeekId == finalWeekId && t.FactoryName == item.FactoryName && t.OrderType == (int)OrderType.Implantation).Count();
                //var listOfImplantation = _appDbContext.Inspections.Where(t => t.DateChecked.Date >= Convert.ToDateTime(blockingRateList.StartDate) && t.DateChecked.Date < Convert.ToDateTime(blockingRateList.EndDate) && t.FactoryName == item.FactoryName && t.OrderType == (int)OrderType.Implantation).Count();

                int numberCheck = 0;
                if (blockingRate <= 5)
                {
                    var rate = Decimal.Divide(1, 16);
                    numberCheck = (int)Math.Round(totalPO * rate);
                    if (numberCheck < listOfImplantation)
                    {
                        numberCheck = listOfImplantation;
                    }
                }
                else if (blockingRate > 5 && blockingRate <= 10)
                {
                    var rate = Decimal.Divide(1, 8);
                    numberCheck = (int)Math.Round(totalPO * rate);
                    if (numberCheck < listOfImplantation)
                    {
                        numberCheck = listOfImplantation;
                    }
                }
                else if (blockingRate > 10 && blockingRate <= 25)
                {
                    var rate = Decimal.Divide(1, 2);
                    numberCheck = (int)Math.Round(totalPO * rate);
                    if (numberCheck < listOfImplantation)
                    {
                        numberCheck = listOfImplantation;
                    }
                }
                else
                {
                    numberCheck = totalPO;
                }               

                numberOfPONeedToCheck.FactoryName = item.FactoryName;
                numberOfPONeedToCheck.BlockingRate = blockingRate;
                numberOfPONeedToCheck.NumberOfPONeedToCheck = numberCheck;
                numberOfPONeedToCheck.TotalPO = totalPO;

                numberOfPONeedToCheckList.Add(numberOfPONeedToCheck);
            }

            return numberOfPONeedToCheckList;
        }

        public DPMListViewModel GetDPMByFactory()
        {

            DayOfWeek weekStart = DayOfWeek.Sunday; // or Sunday, or whenever
            DateTime startingDate = DateTime.Today;

            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            //lấy ngày thứ 2 cách đó 5 tuần
            DateTime previousFiveWeekStart = startingDate.AddDays(-35);

            //tổng số sản phẩm được kiểm tra rồi
            var list = _appDbContext.Inspections.Where(t => t.InspectDate.Date >= previousFiveWeekStart.Date && t.InspectDate.Date < startingDate.Date && t.InspectStatus == true).ToList();

            var manufacturerList = (from so in _appDbContext.Inspections
                                    group so by so.FactoryName into TotaledFactories
                                    select new
                                    {
                                        FactoryName = TotaledFactories.Key,
                                    });
            List<DPMViewModel> dpmList = new List<DPMViewModel>();
            foreach (var item in manufacturerList)
            {
                var checkedItems = list.Where(a => a.FactoryName == item.FactoryName).Select(b => b.NumberChecked).Sum();
                var mistakeItems = list.Where(x => x.FactoryName == item.FactoryName).Select(b => b.ProductQuantityChecked).Sum();

                double dpmValue = checkedItems == 0 ? 0 : Math.Round(((double)mistakeItems / (double)checkedItems) * 1000000, 0);
                DPMViewModel dpm = new DPMViewModel()
                {
                    CheckedItems = checkedItems,
                    MistakeItems = mistakeItems,
                    FactoryName = item.FactoryName,
                    DPMValue = dpmValue
                    //FactoryId = factoryId
                };

                dpmList.Add(dpm);
            }

            DPMListViewModel result = new DPMListViewModel()
            {
                DPMList = dpmList,
                StartDate = previousFiveWeekStart.AddDays(1).ToString("d"),
                EndDate = startingDate.AddDays(-1).ToString("d")
            };

            return result;
        }

        public DPMViewModel GetDPM()
        {

            DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
            DateTime startingDate = DateTime.Today;

            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            DateTime previousFiveWeekStart = startingDate.AddDays(-35);

            //tổng số sản phẩm được kiểm tra rồi
            var list = _appDbContext.Inspections.Where(t => t.InspectDate.Date >= previousFiveWeekStart.Date && t.InspectDate.Date < startingDate.Date && t.InspectStatus == true).ToList();

            var checkedItems = list.Select(b => b.NumberChecked).Sum();
            var mistakeItems = list.Select(b => b.ProductQuantityChecked).Sum();
            double dpmValue = checkedItems == 0 ? 0 : Math.Round(((double)mistakeItems / (double)checkedItems) * 1000000, 0);
            DPMViewModel result = new DPMViewModel()
            {
                CheckedItems = checkedItems,
                MistakeItems = mistakeItems,
                DPMValue = dpmValue
            };

            return result;
        }

        public BRDatapoint GetBlockingRateDatapoint(DateTime startingDate)
        {
            DateTime previousFiveWeekStart = startingDate.AddDays(-35);

            var list = _appDbContext.Inspections.Where(t => t.InspectDate.Date >= previousFiveWeekStart.Date && t.InspectDate.Date < startingDate.Date && t.InspectStatus == true);

            var rejectPO = list.Where(x => x.Result == (int)InspectionResultEnum.Reject).Count();
            var totalPO = list.Count();
            var blockingRate = totalPO == 0 ? 0 : (int)Math.Round((double)(100 * rejectPO) / totalPO);

            var importHelper = new ImportHelper();
            int weekOfYear = 0;
            weekOfYear = importHelper.GetIso8601WeekOfYear(startingDate);

            BRDatapoint result = new BRDatapoint()
            {
                Week = "W" + weekOfYear + "-" + startingDate.Year.ToString(),
                BRValue = blockingRate
            };

            return result;
        }

        public BRChart GetBRChartDatapoint()
        {
            var weekList = new List<DateTime>();
            DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
            DateTime currentMonday = DateTime.Today;

            while (currentMonday.DayOfWeek != weekStart)
                currentMonday = currentMonday.AddDays(-1);

            for(int i = 0; i < 12; i++)
            {
                weekList.Add(currentMonday);
                currentMonday = currentMonday.AddDays(-7);
            }

            var brDatapoints = new List<BRDatapoint>();
            foreach (var item in weekList)
            {
                var brDatapoint = GetBlockingRateDatapoint(item);
                brDatapoints.Add(brDatapoint);
            }

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            var threshold = Convert.ToInt32(Configuration.GetSection("ChartConfiguration:Threshold").Value);
            int maxValue = brDatapoints.Max(x => x.BRValue);
            int top = 25;
            if(maxValue > top)
            {
                top = 50;
            }
            BRChart result = new BRChart()
            {
                BRDatapoints = brDatapoints,
                Threshold = threshold,
                Top = top
            };
            return result;
        }
    }
}
