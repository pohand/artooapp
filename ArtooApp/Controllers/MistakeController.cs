using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Artoo.IRepositories;
using Artoo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Artoo.Models.FileInputModel;
using System.Drawing;
using Artoo.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Artoo.IServices;
using Artoo.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(TenantAttribute))]
    public class MistakeController : Controller
    {
        private readonly IMistakeRepository _mistakeRepository;
        private readonly IMistakeCategoryService _mistakeCategoryService;
        private const int itemsPerPage = 15;
        private Tenant _tenant;

        public MistakeController(IMistakeRepository mistakeRepository,
            IMistakeCategoryService mistakeCategoryService)
        {
            _mistakeRepository = mistakeRepository;
            _mistakeCategoryService = mistakeCategoryService;
            //_tenant = new Tenant();
        }

        public IActionResult Create(string type)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            MistakeViewModel model = new MistakeViewModel();
            List<SelectListItem> mistakeCategories = new List<SelectListItem>();
            if (type == "1")
            {
                mistakeCategories = _mistakeCategoryService.MistakeCategoriesByType(MistakeEnum.DeviceMistake);
                model.MistakeCategoryList = mistakeCategories;
                model.ManualType = 1;
                model.ImageUrl = _tenant?.HostName + "/" + model.ImageUrl;
            }
            else
            {
                mistakeCategories = _mistakeCategoryService.MistakeCategoriesByType(MistakeEnum.ManualMistake);
                model.MistakeCategoryList = mistakeCategories;
                model.ManualType = 0;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MistakeViewModel mistakeVM)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }

            if (ModelState.IsValid)
            {
                var mistake = new Mistake();
                mistake.Description = mistakeVM.Description;
                mistake.ManualType = mistakeVM.ManualType;
                mistake.Name = mistakeVM.Name;
                mistake.Status = mistakeVM.Status;
                mistake.MistakeCategoryID = mistakeVM.SelectedMistakeCategoryId;

                if (mistakeVM.Image != null && mistakeVM.Image.Length != 0)
                {
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages" + "\\" + _tenant?.HostName, mistakeVM.Image.GetFilename());

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await mistakeVM.Image.CopyToAsync(stream);
                    }

                    try
                    {
                        var image = new Bitmap(System.Drawing.Image.FromFile(path));
                        mistake.ImageUrl = mistakeVM.Image.GetFilename();
                        mistakeVM.ImageUrl = mistakeVM.Image.GetFilename();
                    }
                    catch (Exception ex)
                    {
                        var e = ex;
                        Remove(path);
                        ModelState.AddModelError("", "Please update picture with *.jpg or *.png");
                        return View(mistakeVM);
                    }
                }
                
                _mistakeRepository.CreateMistake(mistake);
                ViewBag.message = "Success";
                return RedirectToAction("Index");
            }
            return View(mistakeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Details([FromForm] MistakeViewModel mistakeVM)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            if (ModelState.IsValid)
            {
                var existing = _mistakeRepository.GetMistakeById(mistakeVM.MistakeId);

                var mistake = new Mistake();
                mistake.MistakeId = mistakeVM.MistakeId;
                mistake.Description = mistakeVM.Description;
                mistake.ManualType = mistakeVM.ManualType;
                mistake.Name = mistakeVM.Name;
                mistake.Status = mistakeVM.Status;
                mistake.MistakeCategoryID = mistakeVM.SelectedMistakeCategoryId;

                if (mistakeVM.Image != null && mistakeVM.Image.Length != 0)
                {
                    if (existing.ImageUrl != mistakeVM.Image.GetFilename())
                    {
                        if (existing.ImageUrl != null)
                        {
                            var existingImagePath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages" + "\\" + _tenant?.HostName, existing.ImageUrl);
                            Remove(existingImagePath);
                        }
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages" + "\\" + _tenant?.HostName, mistakeVM.Image.GetFilename());

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await mistakeVM.Image.CopyToAsync(stream);
                        }

                        mistake.ImageUrl = mistakeVM.Image.GetFilename();
                    }
                    else
                    {
                        mistake.ImageUrl = mistakeVM.ImageUrl;
                    }
                }
                _mistakeRepository.UpdateMistake(mistake);
                mistakeVM.DateRegister = existing.DateRegister;
                mistakeVM.ImageUrl = mistake.ImageUrl;
                ViewBag.message = "Success";
                return RedirectToAction("Index");
            }

            return View(mistakeVM);
        }

        public bool Remove(string filename)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }

            if (filename == null)
                return false;

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot\\FileUploads" + "\\" + _tenant?.HostName, filename);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return true;
        }
        public ViewResult Index([FromQuery]string manual = "1", [FromQuery]string device = "1")
        {
            int manualMistakeCurrent = Int32.Parse(manual);

            var manualMistakeCount = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.ManualMistake).Count();
            int manualMistakePageCount = (manualMistakeCount + itemsPerPage - 1) / itemsPerPage;
            IEnumerable<Mistake> manualmistakes = _mistakeRepository.Mistakes.Where(p => p.ManualType == (int)MistakeEnum.ManualMistake)
                   .OrderBy(p => p.MistakeId).Skip((manualMistakeCurrent - 1) * itemsPerPage).Take(itemsPerPage);

            int deviceMistakeCurrent = Int32.Parse(device);

            var deviceMistakeCount = _mistakeRepository.Mistakes.Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake).Count();
            int deviceMistakePageCount = (deviceMistakeCount + itemsPerPage - 1) / itemsPerPage;

            IEnumerable<Mistake> devicemistakes = _mistakeRepository.Mistakes.Where(p => p.ManualType == (int)MistakeEnum.DeviceMistake)
                   .OrderBy(p => p.MistakeId).Skip((deviceMistakeCurrent - 1) * itemsPerPage).Take(itemsPerPage);

            ViewBag.message = TempData["Message"];
            return View(new MistakeListViewModel
            {
                ManualMistakes = manualmistakes,
                ManualMistakePage = new PageViewModel()
                {
                    CurrentPage = Int32.Parse(manual),
                    PageCount = manualMistakePageCount
                },
                DeviceMistakePage = new PageViewModel()
                {
                    CurrentPage = Int32.Parse(device),
                    PageCount = deviceMistakePageCount
                },
                DeviceMistakes = devicemistakes
            });
        }

        public IActionResult Details(int id)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }

            var mistake = _mistakeRepository.GetMistakeById(id);            
            if (mistake == null)
                return NotFound();
            var mistakeCategories = _mistakeCategoryService.MistakeCategories();

            var mistakeVM = new MistakeViewModel()
            {
                MistakeId = mistake.MistakeId,
                Description = mistake.Description,
                ManualType = mistake.ManualType,
                DateRegister = mistake.DateRegister,
                ImageUrl = _tenant?.HostName + "/" + mistake.ImageUrl,
                Name = mistake.Name,
                Status = mistake.Status,
                MistakeCategoryList = mistakeCategories,
                SelectedMistakeCategoryId = mistake.MistakeCategoryID
            };

            return View(mistakeVM);
        }

        public IActionResult Delete(int id)
        {
            var mistake = _mistakeRepository.GetMistakeById(id);
            if (mistake == null)
                return NotFound();

            if(_mistakeRepository.CheckExistingMistakeDetailById(id))
            {
                TempData["Message"] = "This mistake can not be deleted. It is used in the report.";
                return RedirectToAction("Index");
            }

            _mistakeRepository.DeleteMistake(id);
            Remove(mistake.ImageUrl);
            TempData["Message"] = "Success";
            return RedirectToAction("Index");
        }
    }
}
