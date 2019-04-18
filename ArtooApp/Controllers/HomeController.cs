using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Artoo.Models;
using Artoo.ViewModels;
using Artoo.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Artoo.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Artoo.Common;
using Artoo.Infrastructure;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(TenantAttribute))]
    public class HomeController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInspectionRepository _inspectionRepository;
        //private KeyValuePair<string, object> _tenant;
        public HomeController(IDashboardRepository dashboardRepository, UserManager<ApplicationUser> userManager, IInspectionRepository inspectionRepository)
        {
            _dashboardRepository = dashboardRepository;
            _userManager = userManager;
            _inspectionRepository = inspectionRepository;
            //_tenant = RouteData.Values.SingleOrDefault(r => r.Key == "tenant");
        }

        public async Task<IActionResult> Index()
        {
            var blockingRateList = _dashboardRepository.GetBlockingRateByFactory();

            var listPO = _dashboardRepository.GetNumberOfPONeedToCheckListByFactory(blockingRateList);

            int numberPONeedToInspect = 0;

            foreach (var item in listPO)
            {
                numberPONeedToInspect += item.NumberOfPONeedToCheck;
            }

            var numberOfPONeedToCheckList = _dashboardRepository.GetNumberOfPONeedToCheckListByFactory(blockingRateList);

            var dpm = _dashboardRepository.GetDPM();
            var dpmList = _dashboardRepository.GetDPMByFactory();
            var brChart = _dashboardRepository.GetBRChartDatapoint();
            var dashboardViewModel = new DashboardViewModel()
            {
                BlokingRate = _dashboardRepository.GetBlockingRate(),
                NumberPONeedToInspect = numberPONeedToInspect,
                BlockingRateList = blockingRateList,
                NumberOfPONeedToCheckList = numberOfPONeedToCheckList,
                DPM = dpm,
                DPMList = dpmList,
                BlockingRateChart = brChart
            };

            var data = string.Empty;
            var week = string.Empty;
            foreach (var item in brChart.BRDatapoints.Reverse())
            {
                data += item.BRValue + ",";
                week += item.Week + ",";
            }

            data = data.Remove(data.Length - 1);
            week = week.Remove(week.Length - 1);

            ViewBag.Data = data;
            ViewBag.Label = week;

            var factoryLabels = string.Empty;
            var poData = string.Empty;
            foreach (var item in numberOfPONeedToCheckList)
            {
                factoryLabels += item.FactoryName + ",";
                poData += item.NumberOfPONeedToCheck + ",";
            }

            if (!string.IsNullOrEmpty(factoryLabels))
            {
                factoryLabels = factoryLabels.Remove(factoryLabels.Length - 1);
                poData = poData.Remove(poData.Length - 1);

                ViewBag.PoData = poData;
                ViewBag.FactoryLabels = factoryLabels;
            }

            var dpmFactoryLabels = string.Empty;
            var dpmData = string.Empty;
            foreach (var item in dpmList.DPMList)
            {
                dpmFactoryLabels += item.FactoryName + ",";
                dpmData += item.DPMValue + ",";
            }

            if (!string.IsNullOrEmpty(dpmData))
            {
                dpmFactoryLabels = dpmFactoryLabels.Remove(dpmFactoryLabels.Length - 1);
                dpmData = dpmData.Remove(dpmData.Length - 1);

                ViewBag.DPMData = dpmData;
                ViewBag.DPMFactoryLabels = dpmFactoryLabels;
            }
            var username = await _userManager.GetUserAsync(User);
            var inspectionList = _inspectionRepository.Inspections;
            if (username != null)
            {
                inspectionList = inspectionList.Where(x => x.UserBookingId == username.Id);
            }
            var list = inspectionList.Where(p => p.Result == (int)InspectionResultEnum.Reject && p.InspectStatus == true);
            var count = list.Count();

            TempData["RejectCount"] = count;
            return View(dashboardViewModel);
        }

        public ViewResult BlockingRate()
        {
            var result = _dashboardRepository.GetBlockingRateByFactory();

            return View(result);
        }

        public ViewResult POCheck()
        {
            var blockingRateList = _dashboardRepository.GetBlockingRateByFactory();

            var result = _dashboardRepository.GetNumberOfPONeedToCheckListByFactory(blockingRateList);

            return View(result);
        }
    }
}
