using Artoo.Common;
using Artoo.Models;
using Artoo.IRepositories;
using Artoo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
//using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Artoo.Helpers;

namespace Artoo.Controllers
{
    [Authorize]
    public class InspectionController : Controller
    {
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IMistakeRepository _mistakeRepository;
        private readonly IPassionBrandRepository _passionBrandRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFinalWeekRepository _finalWeekRepository;
        private readonly IFactoryRepository _factoryRepository;
        private readonly IEmailRepository _emailRepository;

        private const int itemsPerPage = 20;
        public InspectionController(IInspectionRepository inspectionRepository,
            IMistakeRepository mistakeRepository,
            IPassionBrandRepository passionBrandRepository,
            UserManager<ApplicationUser> userManager,
            IFinalWeekRepository finalWeekRepository,
            IFactoryRepository factoryRepository,
            IEmailRepository emailRepository)
        {
            _inspectionRepository = inspectionRepository;
            _mistakeRepository = mistakeRepository;
            _passionBrandRepository = passionBrandRepository;
            _userManager = userManager;
            _finalWeekRepository = finalWeekRepository;
            _factoryRepository = factoryRepository;
            _emailRepository = emailRepository;
        }

        public async Task<ViewResult> Index(string factoryString, string orderString, string weekString, string techManagerString, int page = 1)
        {
            try
            {
                var username = await _userManager.GetUserAsync(User);
                var inspectionList = _inspectionRepository.Inspections;
                var userList = _userManager.Users;
                int factoryInt = 0;
                //if (User.IsInRole("QPL"))
                //{
                //    if (user.Result.FactoryId != null)
                //    {
                //        inspectionList = inspectionList.Where(x => x.FactoryId == user.Result.FactoryId);
                //    }
                //    //remove after asking about logic
                //    else
                //    {
                //        if (!string.IsNullOrEmpty(factoryString))
                //        {
                //            if (Int32.TryParse(factoryString, out factoryInt))
                //            {
                //                if (factoryInt != 0)
                //                {
                //                    inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(factoryString))
                //    {
                //        if (Int32.TryParse(factoryString, out factoryInt))
                //        {
                //            if (factoryInt != 0)
                //            {
                //                inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                //            }
                //        }
                //    }
                //}

                if (!string.IsNullOrEmpty(factoryString))
                {
                    if (Int32.TryParse(factoryString, out factoryInt))
                    {
                        if (factoryInt != 0)
                        {
                            inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                        }
                    }
                }

                int weekInt = 0;
                if (!string.IsNullOrEmpty(weekString))
                {
                    if (Int32.TryParse(weekString, out weekInt))
                    {
                        if (weekInt != 0)
                        {
                            inspectionList = inspectionList.Where(x => x.FinalWeekId == weekInt);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(orderString))
                {
                    inspectionList = inspectionList.Where(x => x.OrderNumber.IndexOf(orderString, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (!string.IsNullOrEmpty(techManagerString))
                {
                    inspectionList = inspectionList.Where(x => x.TechManagerName.IndexOf(techManagerString, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                var waitingList = inspectionList.Where(x => x.InspectStatus == false && x.Result == 0);
                var count = waitingList.Count();
                int pageCount = (count + itemsPerPage - 1) / itemsPerPage;
                IEnumerable<Inspection> inspections;
                if (count > itemsPerPage)
                {
                    inspections = waitingList
                           .OrderBy(p => p.FinalDate).OrderByDescending(p => p.OrderType).Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage);
                }
                else
                {
                    inspections = waitingList;
                }

                var inspectionListVM = new List<InspectionViewModel>();
                //int itemIndex = page == 1 ? page : ((page * itemsPerPage) - (itemsPerPage -1));
                int itemIndex = ((page * itemsPerPage) - (itemsPerPage - 1));

                foreach (var inspection in inspections)
                {
                    string userBooking = string.Empty;
                    if (!string.IsNullOrEmpty(inspection.UserBookingId))
                    {
                        //var user = _userManager.FindByIdAsync(inspection.UserBookingId);
                        if (username.Id == inspection.UserBookingId)
                        {
                            userBooking = null;
                        }
                        else
                        {
                            userBooking = userList.FirstOrDefault(x => x.Id == inspection.UserBookingId).UserName;
                        }
                    }
                    var inspectionVM = new InspectionViewModel()
                    {
                        ItemIndex = itemIndex++,
                        InspectionId = inspection.InspectionId,
                        Description = inspection.Description,
                        IMAN = inspection.IMAN,
                        InspectDate = inspection.InspectDate,
                        InspectStatus = inspection.InspectStatus,
                        Model = inspection.Model,
                        NumberChecked = inspection.NumberChecked,
                        OrderNumber = inspection.OrderNumber,
                        OrderQuantity = inspection.OrderQuantity,
                        OrderTypeName = inspection.OrderType == 1 ? OrderType.Implantation.ToString() : OrderType.Replenishment.ToString(),
                        OrderType = inspection.OrderType,
                        Parameter = inspection.Parameter,
                        PassionBrandName = inspection.PassionBrandName,
                        PONumber = inspection.PONumber,
                        Username = inspection.Username,
                        FactoryName = inspection.FactoryName,
                        FinalDate = inspection.FinalDate.ToString("d"),
                        BookingStatus = inspection.BookingStatus,
                        TechManagerName = inspection.TechManagerName,
                        UserBookingId = inspection.UserBookingId,
                        UserBooking = userBooking
                    };

                    inspectionListVM.Add(inspectionVM);
                }

                var finalWeekList = GetSelectListFinalWeeks();
                var factoryList = GetSelectListFactories();

                var nextPage = page == pageCount ? page : (page + 1);
                var previousPage = page > 1 ? (page - 1) : page;

                return View(new InspectionListViewModel
                {
                    InspectionList = inspectionListVM,

                    Page = new PageViewModel
                    {
                        CurrentPage = page,
                        PageCount = pageCount,
                        NextPage = nextPage,
                        PreviousPage = previousPage
                    },
                    FinalWeekList = finalWeekList,
                    FinalWeekSearchString = weekInt.ToString(),
                    FactorySearchString = factoryInt.ToString(),
                    OrderNumberSearchString = orderString,
                    TechManagerNameSearchString = techManagerString,
                    FactoryList = factoryList
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ViewResult> List(string factoryString, string orderString, string weekString, string techManagerString, int page = 1)
        {
            var username = await _userManager.GetUserAsync(User);
            var inspectionList = _inspectionRepository.Inspections;
            if (username != null)
            {
                inspectionList = inspectionList.Where(x => x.UserBookingId == username.Id);
            }

            int factoryInt = 0;
            if (!string.IsNullOrEmpty(factoryString))
            {
                if (Int32.TryParse(factoryString, out factoryInt))
                {
                    if (factoryInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                    }
                }
            }

            int weekInt = 0;
            if (!string.IsNullOrEmpty(weekString))
            {
                if (Int32.TryParse(weekString, out weekInt))
                {
                    if (weekInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FinalWeekId == weekInt);
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderString))
            {
                inspectionList = inspectionList.Where(x => x.OrderNumber.IndexOf(orderString, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(techManagerString))
            {
                inspectionList = inspectionList.Where(x => x.TechManagerName.IndexOf(techManagerString, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var count = inspectionList.Where(p => p.BookingStatus == true && p.InspectStatus == false).Count();
            int pageCount = (count + itemsPerPage - 1) / itemsPerPage;
            IEnumerable<Inspection> inspections = inspectionList.Where(p => p.BookingStatus == true && p.InspectStatus == false)
                   .OrderByDescending(p => p.InspectionId).Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage);

            var inspectionListVM = new List<InspectionViewModel>();

            foreach (var inspection in inspections)
            {
                var inspectionVM = new InspectionViewModel()
                {
                    InspectionId = inspection.InspectionId,
                    Description = inspection.Description,
                    IMAN = inspection.IMAN,
                    InspectDate = inspection.InspectDate,
                    InspectStatus = inspection.InspectStatus,
                    Model = inspection.Model,
                    NumberChecked = inspection.NumberChecked,
                    OrderNumber = inspection.OrderNumber,
                    OrderQuantity = inspection.OrderQuantity,
                    OrderTypeName = inspection.OrderType == 1 ? "Implantation" : "Replenishment",
                    Parameter = inspection.Parameter,
                    PassionBrandName = inspection.PassionBrandName,
                    PONumber = inspection.PONumber,
                    Username = inspection.Username,
                    FactoryName = inspection.FactoryName,
                    FinalDate = inspection.FinalDate.ToString("d"),
                    TechManagerName = inspection.TechManagerName,
                    BookingStatus = inspection.BookingStatus
                };

                var manualMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.ManualMistake).ToList();

                var deviceMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake).ToList();

                inspectionVM.ManualMistakeList = manualMistakeList;
                inspectionVM.DeviceMistakeList = deviceMistakeList;
                inspectionListVM.Add(inspectionVM);
            }

            //Start Search options
            var finalWeekList = GetSelectListFinalWeeks();
            var factoryList = GetSelectListFactories();

            var nextPage = page == pageCount ? page : (page + 1);
            var previousPage = page > 1 ? (page - 1) : page;

            return View(new InspectionListViewModel
            {
                InspectionList = inspectionListVM,

                Page = new PageViewModel
                {
                    CurrentPage = page,
                    PageCount = pageCount,
                    NextPage = nextPage,
                    PreviousPage = previousPage
                },
                FinalWeekList = finalWeekList,
                FinalWeekSearchString = weekInt.ToString(),
                FactorySearchString = factoryInt.ToString(),
                OrderNumberSearchString = orderString,
                TechManagerNameSearchString = techManagerString,
                FactoryList = factoryList
            });
            //End Search options
        }
        public ViewResult Report(string factoryString, string orderString, string weekString, string techManagerString, string resultString, int page = 1)
        {
            var inspectionList = _inspectionRepository.Inspections;
            int factoryInt = 0;
            if (!string.IsNullOrEmpty(factoryString))
            {
                if (Int32.TryParse(factoryString, out factoryInt))
                {
                    if (factoryInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                    }
                }
            }

            int weekInt = 0;
            if (!string.IsNullOrEmpty(weekString))
            {
                if (Int32.TryParse(weekString, out weekInt))
                {
                    if (weekInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FinalWeekId == weekInt);
                    }
                }
            }

            int resultInt = 0;
            if (!string.IsNullOrEmpty(resultString))
            {
                if (Int32.TryParse(resultString, out resultInt))
                {
                    if (resultInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.Result == resultInt);
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderString))
            {
                inspectionList = inspectionList.Where(x => x.OrderNumber.IndexOf(orderString, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(techManagerString))
            {
                //inspectionList = inspectionList.Where(x => x.TechManagerName.IndexOf(techManagerString, StringComparison.OrdinalIgnoreCase) >= 0);
                inspectionList = inspectionList.Where(x => x.TechManagerName.Contains(techManagerString, StringComparison.OrdinalIgnoreCase));

            }
            var count = inspectionList.Where(p => p.BookingStatus == true && p.InspectStatus == true).Count();
            int pageCount = (count + itemsPerPage - 1) / itemsPerPage;
            IEnumerable<Inspection> inspections = inspectionList.Where(p => p.BookingStatus == true && p.InspectStatus == true)
                   .OrderByDescending(p => p.InspectDate).Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage);

            var inspectionListVM = new List<InspectionViewModel>();

            foreach (var inspection in inspections)
            {
                bool rdParty = false;
                if (inspection.IsThirdParty == null)
                {
                    rdParty = false;
                }
                else
                {
                    rdParty = inspection.IsThirdParty == false ? false : true;
                }
                var inspectionVM = new InspectionViewModel()
                {
                    InspectionId = inspection.InspectionId,
                    Description = inspection.Description,
                    IMAN = inspection.IMAN,
                    InspectDate = inspection.InspectDate,
                    InspectStatus = inspection.InspectStatus,
                    Model = inspection.Model,
                    NumberChecked = inspection.NumberChecked,
                    OrderNumber = inspection.OrderNumber,
                    OrderQuantity = inspection.OrderQuantity,
                    OrderTypeName = inspection.OrderType == 1 ? OrderType.Implantation.ToString() : OrderType.Replenishment.ToString(),
                    Parameter = inspection.Parameter,
                    PassionBrandName = inspection.PassionBrandName,
                    PONumber = inspection.PONumber,
                    Username = inspection.Username,
                    FactoryName = inspection.FactoryName,
                    FinalDate = inspection.FinalDate.ToString("d"),
                    ResultString = inspection.Result == (int)InspectionResultEnum.Accept ? InspectionResultEnum.Accept.ToString() : InspectionResultEnum.Reject.ToString(),
                    Result = inspection.Result,
                    ApproveUsername = inspection.ApproveUsername,
                    TechManagerName = inspection.TechManagerName,
                    IsThirdParty = rdParty
                };

                var manualMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.ManualMistake).ToList();

                var deviceMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake).ToList();

                inspectionVM.ManualMistakeList = manualMistakeList;
                inspectionVM.DeviceMistakeList = deviceMistakeList;
                inspectionListVM.Add(inspectionVM);
            }

            //Start Search options
            var finalWeekList = GetSelectListFinalWeeks();
            var factoryList = GetSelectListFactories();
            var resultList = GetSelectListResults();

            var nextPage = page == pageCount ? page : (page + 1);
            var previousPage = page > 1 ? (page - 1) : page;

            return View(new InspectionListViewModel
            {
                InspectionList = inspectionListVM,

                Page = new PageViewModel
                {
                    CurrentPage = page,
                    PageCount = pageCount,
                    NextPage = nextPage,
                    PreviousPage = previousPage
                },
                FinalWeekList = finalWeekList,
                FinalWeekSearchString = weekInt.ToString(),
                FactorySearchString = factoryInt.ToString(),
                OrderNumberSearchString = orderString,
                TechManagerNameSearchString = techManagerString,
                FactoryList = factoryList,
                ResultList = resultList,
                ResultSearchString = resultString
            });
            //End Search options
        }

        public List<SelectListItem> GetSelectListFinalWeeks()
        {
            return _finalWeekRepository.FinalWeeks.OrderByDescending(x => x.Year).OrderByDescending(x => x.Week)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.FinalWeekId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetSelectListFactories()
        {
            return _factoryRepository.Factories.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.FactoryId.ToString()
            }).ToList();
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(User);
        }

        public List<SelectListItem> GetSelectListResults()
        {
            var results = Enum.GetValues(typeof(InspectionResultEnum)).Cast<InspectionResultEnum>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            //var initial = new SelectListItem()
            //{
            //    Text = "All",
            //    Value = "0"
            //};

            //results.Add(initial);
            return results;
        }

        public IActionResult Book(int bookid, string factoryString, string orderString, string weekString, string techManagerString, int page)
        {
            var inspection = _inspectionRepository.GetInspectionById(bookid);
            if (inspection == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            _inspectionRepository.BookInspection(bookid, userId);
            return RedirectToAction("Index", new { page, factoryString, orderString, weekString, techManagerString });

        }

        public IActionResult UnBook(int bookid, string factoryString, string orderString, string weekString, string techManagerString, int page)
        {
            var inspection = _inspectionRepository.GetInspectionById(bookid);
            if (inspection == null)
                return NotFound();

            _inspectionRepository.UnBookInspection(bookid);
            return RedirectToAction("Index", new { page, factoryString, orderString, weekString, techManagerString });

        }

        public IActionResult UnBookToList(int bookid, string factoryString, string orderString, string weekString, string techManagerString, int page)
        {
            var inspection = _inspectionRepository.GetInspectionById(bookid);
            if (inspection == null)
                return NotFound();

            _inspectionRepository.UnBookInspection(bookid);
            return RedirectToAction("List", new { page, factoryString, orderString, weekString, techManagerString });

        }

        public ViewResult Details(int id)
        {
            var existing = _inspectionRepository.GetInspectionById(id);

            var manualMistake = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.ManualMistake)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.MistakeId.ToString()
                }).ToList();

            var deviceMistake = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.MistakeId.ToString()
                }).ToList();

            var passionBrands = _passionBrandRepository.PassionBrands
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.PassionBrandId.ToString()
                }).ToList();

            int numberChecked = 1;
            switch (existing.OrderQuantity)
            {
                case int n when (n > 2 && n <= 15):
                    numberChecked = 2;
                    break;
                case int n when (n > 16 && n <= 25):
                    numberChecked = 3;
                    break;
                case int n when (n > 26 && n <= 90):
                    numberChecked = 5;
                    break;
                case int n when (n > 91 && n <= 150):
                    numberChecked = 8;
                    break;
                case int n when (n > 151 && n <= 500):
                    numberChecked = 13;
                    break;
                case int n when (n > 501 && n <= 1200):
                    numberChecked = 20;
                    break;
                case int n when (n > 1201 && n <= 10000):
                    numberChecked = 32;
                    break;
                case int n when (n > 10001):
                    numberChecked = 50;
                    break;
                default:
                    numberChecked = 1;
                    break;
            }
            bool rdParty = false;
            if (existing.IsThirdParty == null)
            {
                rdParty = false;
            }
            else
            {
                rdParty = existing.IsThirdParty == false ? false : true;
            }
            InspectionViewModel inspectionVM = new InspectionViewModel()
            {
                InspectionId = existing.InspectionId,
                Username = User.Identity.Name,
                DateChecked = DateTime.Now,
                PassionBrandName = existing.PassionBrandName,
                IMAN = existing.IMAN,
                Model = existing.Model,
                OrderNumber = existing.OrderNumber,
                FactoryName = existing.FactoryName,
                OrderType = existing.OrderType,
                OrderQuantity = existing.OrderQuantity,
                OrderTypeName = existing.OrderType == 1 ? "Implantation" : "Replenishment",
                FinalDate = existing.FinalDate.ToString("d"),
                ManualMistake = manualMistake,
                DeviceMistake = deviceMistake,
                PassionBrands = passionBrands,
                NumberChecked = numberChecked,
                PassionBrandId = existing.PassionBrandId,
                TechManagerName = existing.TechManagerName,
                Description = existing.Description,
                IsThirdParty = rdParty
            };
            return View(inspectionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Details(InspectionViewModel inspectionVM)
        {
            var inspection = new Inspection();
            var username = User.Identity.Name;
            inspection.InspectionId = inspectionVM.InspectionId;
            DateTime rdDate;
            if (inspectionVM.NumberChecked == 0)
            {
                ModelState.AddModelError("NumberChecked", "Please input Số Lượng Kiểm");
            }
            if (inspectionVM.Result == 0)
            {
                ModelState.AddModelError("Result", "Please select Result");
            }
            if (inspectionVM.ProductQuantityChecked == null)
            {
                ModelState.AddModelError("ProductQuantityChecked", "Please input Số lượng sản phẩm lỗi");
            }
            if (inspectionVM.IsThirdParty == true && string.IsNullOrEmpty(inspectionVM.ThirdPartyDate))
            {
                ModelState.AddModelError("ThirdPartyDate", "Please input Third Party Date");
            }
            else if (inspectionVM.IsThirdParty == true && !string.IsNullOrEmpty(inspectionVM.ThirdPartyDate))
            {
                if (!DateTime.TryParse(inspectionVM.ThirdPartyDate, out rdDate))
                {
                    ModelState.AddModelError("ThirdPartyDate", "Please input Third Party Date format correctly");
                }
            }
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(inspectionVM.ManualMistakeString))
                {
                    var mistakeString = inspectionVM.ManualMistakeString.Split('|');
                    var mistakeList = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.ManualMistake);
                    foreach (var item in mistakeString)
                    {
                        var quantity = Regex.Match(item, @"\{([^)]*)\}").Groups[1].Value;

                        int numValue;
                        bool parsed = Int32.TryParse(quantity, out numValue);

                        if (!parsed)
                        {
                            numValue = 0;
                        }

                        string regex = "(\\[.*\\])|(\\{.*\\})";
                        string mistakeName = Regex.Replace(item, regex, "");

                        var checkMistakeId = mistakeList.Any(x => x.Name == mistakeName);
                        if (checkMistakeId)
                        {
                            var mistakeId = mistakeList.FirstOrDefault(x => x.Name == mistakeName).MistakeId;
                            var checkExistingInMatching = _inspectionRepository.InspectionMistakeDetails.Any(x => x.MistakeId == mistakeId && x.InspectionId == inspectionVM.InspectionId);
                            if (!checkExistingInMatching)
                            {
                                var imd = new InspectionMistakeDetail()
                                {
                                    InspectionId = inspectionVM.InspectionId,
                                    MistakeId = mistakeId,
                                    Quantity = numValue
                                };

                                _inspectionRepository.AddInspectionMistakeDetail(imd);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(inspectionVM.DeviceMistakeString))
                {
                    var mistakeString = inspectionVM.DeviceMistakeString.Split('|');
                    var mistakeList = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake);
                    foreach (var item in mistakeString)
                    {
                        var quantity = Regex.Match(item, @"\(([^)]*)\)").Groups[1].Value;
                        int numValue;
                        bool parsed = Int32.TryParse(quantity, out numValue);

                        if (!parsed)
                        {
                            numValue = 0;
                        }

                        var checkMistakeId = mistakeList.Any(x => x.Name == item);
                        if (checkMistakeId)
                        {
                            var mistakeId = mistakeList.FirstOrDefault(x => x.Name == item).MistakeId;
                            var checkExistingInMatching = _inspectionRepository.InspectionMistakeDetails.Any(x => x.MistakeId == mistakeId && x.InspectionId == inspectionVM.InspectionId);
                            if (!checkExistingInMatching)
                            {
                                var imd = new InspectionMistakeDetail()
                                {
                                    InspectionId = inspectionVM.InspectionId,
                                    MistakeId = mistakeId,
                                    Quantity = numValue
                                };
                                _inspectionRepository.AddInspectionMistakeDetail(imd);
                            }
                        }
                    }
                }

                inspection.Parameter = inspectionVM.Parameter;
                inspection.PassionBrandId = inspectionVM.PassionBrandId;
                inspection.CustomerComment = inspectionVM.CustomerComment;
                inspection.FactoryComment = inspectionVM.FactoryComment;
                inspection.NumberChecked = inspectionVM.NumberChecked;
                inspection.ProductQuantityChecked = inspectionVM.ProductQuantityChecked;
                inspection.IsThirdParty = inspectionVM.IsThirdParty;
                
                //First of all, saving inspectdate when user selects third party checkbox.
                //However, there is a minor change to allow to save inspect date as long as available thirdpartydate.
                if (!string.IsNullOrEmpty(inspectionVM.ThirdPartyDate))
                {
                    inspection.InspectDate = DateTime.Parse(inspectionVM.ThirdPartyDate);
                }
                else
                {
                    inspection.InspectDate = DateTime.Now;
                }
                inspection.Result = inspectionVM.Result;
                inspection.InspectStatus = true;
                inspection.Username = username;
                _inspectionRepository.UpdateInspection(inspection);


                if (inspectionVM.PassionBrandId > 0 && inspection.Result > 0)
                {
                    var mailList = _emailRepository.GetEmailByBrandResult(inspectionVM.PassionBrandId, (InspectionResultEnum)inspection.Result).ToList();
                    var manualMistakeString = string.Empty;
                    if (inspectionVM.ManualMistakeString != null)
                    {
                        manualMistakeString = inspectionVM.ManualMistakeString.Replace("{", "(").Replace("}", ")").Replace("|", "; ");
                    }
                    var deviceMistakeString = string.Empty;
                    if (inspectionVM.DeviceMistakeString != null)
                    {
                        deviceMistakeString = inspectionVM.DeviceMistakeString.Replace("{", "(").Replace("}", ")").Replace("|", "; ");
                    }
                    if (mailList != null)
                    {
                        if (mailList.Count > 0)
                        {
                            var mailbody = $@"Gửi anh chị quản lý chất lượng và quản lý xưởng,
                            Đây là kết quả Final Inspection do QPL đã thực hiện.<br />
                            Nhà máy vui lòng xác nhận bằng cách reply mail này.
                            <table style='border-radius:2px;margin:20px 0 25px;min-width:400px;border:1px solid #eee' cellspacing='0' cellpadding='10' border='0'>
	                            <tbody>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Người kiểm</td>
			                            <td style='border-bottom:1px solid #eee'>{inspection.Username}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Ngày kiểm</td>
			                            <td style='border-bottom:1px solid #eee'>{inspection.InspectDate}</td>
		                            </tr>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Passion Brand</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.PassionBrandName}</td>
		                            </tr>
			                            <tr style='background:#f9f9f9'><td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>IMAN</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.IMAN}</td>
		                            </tr>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Model</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.Model}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Số PO</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.OrderNumber}</td>
		                            </tr>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Số lượng kiểm</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.NumberChecked}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Xưởng sản xuất</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.FactoryName}</td>
		                            </tr>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Loại đơn hàng</td>
			                            <td style='border-bottom:1px solid #eee'>{((OrderType)inspectionVM.OrderType).ToString()}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Kiểm tra bằng tay và mắt</td>
			                            <td style='border-bottom:1px solid #eee'>{manualMistakeString}</td>
		                            </tr>
		                            <tr style='background:#fbfbfb'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Kiểm tra bằng thiết bị </td>
			                            <td style='border-bottom:1px solid #eee'>{deviceMistakeString}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Thông số</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.Parameter}</td>
		                            </tr>		
                                    <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Khách hàng góp ý</td>
			                            <td style='border-bottom:1px solid #eee'>{inspectionVM.CustomerComment}</td>
		                            </tr>
		                            <tr style='background:#f9f9f9'>
			                            <td style='font-weight:bold;border-bottom:1px solid #eee;width:180px'>Result</td>
			                            <td style='border-bottom:1px solid #eee'>{((InspectionResultEnum)inspectionVM.Result).ToString()}</td>
		                            </tr>		
	                            </tbody>
                            </table>";

                            EmailSender sender = new EmailSender();
                            var subject = $@"[{((OrderType)inspectionVM.OrderType).ToString()}] Kết quả kiểm Final: {inspectionVM.PassionBrandName} - [{((InspectionResultEnum)inspectionVM.Result).ToString()}] - IMAN: {inspectionVM.IMAN} - Model: {inspectionVM.Model} - PO: {inspectionVM.OrderNumber}";
                            await sender.SendEmailAsync(mailList.Select(x => x.EmailAddress).ToList(), subject, mailbody);
                        }
                    }
                }
                return RedirectToAction("List");
            }

            return View(inspectionVM);
        }
        public IActionResult Delete(int id)
        {
            var mistake = _inspectionRepository.GetInspectionById(id);
            if (mistake == null)
                return NotFound();

            _inspectionRepository.DeleteInspection(id);
            return RedirectToAction("Index");
        }

        public IActionResult ReportApprove(int id, string factoryString, string orderString, string weekString, string techManagerString, string resultString, string pageTotal, int page = 1)
        {
            var existing = _inspectionRepository.GetInspectionById(id);
            if (existing == null)
                return NotFound();

            var username = User.Identity.Name;
            if (!string.IsNullOrEmpty(existing.ApproveUsername))
            {
                existing.ApproveUsername = string.Empty;
            }
            else
            {
                existing.ApproveUsername = username;
            }

            _inspectionRepository.UpdateInspection(existing);
            //int pageCount = 0;
            //if(Int32.TryParse(pageTotal, out pageCount))
            //{
            //    var finalWeekList = GetSelectListFinalWeeks();
            //    var factoryList = GetSelectListFactories();
            //    var resultList = GetSelectListResults();

            //    var nextPage = page == pageCount ? page : (page + 1);
            //    var previousPage = page > 1 ? (page - 1) : page;
            //    var inspectionListVM = new InspectionListViewModel
            //    {
            //        Page = new PageViewModel
            //        {
            //            CurrentPage = page,
            //            PageCount = pageCount,
            //            NextPage = nextPage,
            //            PreviousPage = previousPage
            //        },
            //        FinalWeekList = finalWeekList,
            //        FinalWeekSearchString = weekString,
            //        FactorySearchString = factoryString,
            //        OrderNumberSearchString = orderString,
            //        TechManagerNameSearchString = techManagerString,
            //        FactoryList = factoryList,
            //        ResultList = resultList,
            //        ResultSearchString = resultString
            //    };
            //    return RedirectToAction("Report", inspectionListVM);
            //}

            return RedirectToAction("Report", new { page, factoryString, orderString, weekString, techManagerString });
        }

        public JsonResult GetManualMistakeByPrefix(string prefix)
        {
            var mistakes = _mistakeRepository.GetMistakeByPrefix(prefix, MistakeEnum.ManualMistake);
            return Json(mistakes);
        }

        public JsonResult GetDeviceMistakeByPrefix(string prefix)
        {
            var mistakes = _mistakeRepository.GetMistakeByPrefix(prefix, MistakeEnum.DeviceMistake);
            return Json(mistakes);
        }

        public async Task<ViewResult> RejectList(string factoryString, string orderString, string weekString, string techManagerString, int page = 1)
        {
            var username = await _userManager.GetUserAsync(User);
            var inspectionList = _inspectionRepository.Inspections;
            if (username != null)
            {
                inspectionList = inspectionList.Where(x => x.UserBookingId == username.Id);
            }

            int factoryInt = 0;
            if (!string.IsNullOrEmpty(factoryString))
            {
                if (Int32.TryParse(factoryString, out factoryInt))
                {
                    if (factoryInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FactoryId == factoryInt);
                    }
                }
            }

            int weekInt = 0;
            if (!string.IsNullOrEmpty(weekString))
            {
                if (Int32.TryParse(weekString, out weekInt))
                {
                    if (weekInt != 0)
                    {
                        inspectionList = inspectionList.Where(x => x.FinalWeekId == weekInt);
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderString))
            {
                inspectionList = inspectionList.Where(x => x.OrderNumber.IndexOf(orderString, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(techManagerString))
            {
                inspectionList = inspectionList.Where(x => x.TechManagerName.IndexOf(techManagerString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            var list = inspectionList.Where(p => p.Result == (int)InspectionResultEnum.Reject && p.InspectStatus == true);
            var count = list.Count();
            int pageCount = (count + itemsPerPage - 1) / itemsPerPage;
            IEnumerable<Inspection> inspections = list
                   .OrderByDescending(p => p.InspectionId).Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage);

            var inspectionListVM = new List<InspectionViewModel>();

            foreach (var inspection in inspections)
            {
                var inspectionVM = new InspectionViewModel()
                {
                    InspectionId = inspection.InspectionId,
                    Description = inspection.Description,
                    IMAN = inspection.IMAN,
                    InspectDate = inspection.InspectDate,
                    InspectStatus = inspection.InspectStatus,
                    Model = inspection.Model,
                    NumberChecked = inspection.NumberChecked,
                    OrderNumber = inspection.OrderNumber,
                    OrderQuantity = inspection.OrderQuantity,
                    OrderTypeName = inspection.OrderType == 1 ? "Implantation" : "Replenishment",
                    Parameter = inspection.Parameter,
                    PassionBrandName = inspection.PassionBrandName,
                    PONumber = inspection.PONumber,
                    Username = inspection.Username,
                    FactoryName = inspection.FactoryName,
                    FinalDate = inspection.FinalDate.ToString("d"),
                    TechManagerName = inspection.TechManagerName,
                    BookingStatus = inspection.BookingStatus
                };

                var manualMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.ManualMistake).ToList();

                var deviceMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake).ToList();

                inspectionVM.ManualMistakeList = manualMistakeList;
                inspectionVM.DeviceMistakeList = deviceMistakeList;
                inspectionListVM.Add(inspectionVM);
            }

            //Start Search options
            var finalWeekList = GetSelectListFinalWeeks();
            var factoryList = GetSelectListFactories();

            var nextPage = page == pageCount ? page : (page + 1);
            var previousPage = page > 1 ? (page - 1) : page;

            return View(new InspectionListViewModel
            {
                InspectionList = inspectionListVM,

                Page = new PageViewModel
                {
                    CurrentPage = page,
                    PageCount = pageCount,
                    NextPage = nextPage,
                    PreviousPage = previousPage
                },
                FinalWeekList = finalWeekList,
                FinalWeekSearchString = weekInt.ToString(),
                FactorySearchString = factoryInt.ToString(),
                OrderNumberSearchString = orderString,
                TechManagerNameSearchString = techManagerString,
                FactoryList = factoryList
            });
            //End Search options
        }

    }
}
