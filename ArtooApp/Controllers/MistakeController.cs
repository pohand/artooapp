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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    public class MistakeController : Controller
    {
        private readonly IMistakeRepository _mistakeRepository;
        private const int itemsPerPage = 15;

        public MistakeController(IMistakeRepository mistakeRepository)
        {
            _mistakeRepository = mistakeRepository;
        }

        public IActionResult Create(string type)
        {
            MistakeViewModel model = new MistakeViewModel();
            if (type == "1")
            {
                model.ManualType = 1;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MistakeViewModel mistakeVM)
        {
            if (ModelState.IsValid)
            {
                var mistake = new Mistake();
                mistake.Description = mistakeVM.Description;
                mistake.ManualType = mistakeVM.ManualType;
                mistake.Name = mistakeVM.Name;
                mistake.Status = mistakeVM.Status;

                if (mistakeVM.Image != null && mistakeVM.Image.Length != 0)
                {
                    var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages", mistakeVM.Image.GetFilename());

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
            }
            return View(mistakeVM);
        }

        [HttpPost]
        public async Task<IActionResult> Details([FromForm] MistakeViewModel mistakeVM)
        {
            if (ModelState.IsValid)
            {
                var existing = _mistakeRepository.GetMistakeById(mistakeVM.MistakeId);

                var mistake = new Mistake();
                mistake.MistakeId = mistakeVM.MistakeId;
                mistake.Description = mistakeVM.Description;
                mistake.ManualType = mistakeVM.ManualType;
                mistake.Name = mistakeVM.Name;
                mistake.Status = mistakeVM.Status;

                if (mistakeVM.Image != null && mistakeVM.Image.Length != 0)
                {
                    if (existing.ImageUrl != mistakeVM.Image.GetFilename())
                    {
                        if (existing.ImageUrl != null)
                        {
                            var existingImagePath = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages", existing.ImageUrl);
                            Remove(existingImagePath);
                        }
                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\ErrorImages", mistakeVM.Image.GetFilename());

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
            }

            return View(mistakeVM);
        }

        public bool Remove(string filename)
        {
            if (filename == null)
                return false;

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot\\FileUploads", filename);

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
            var mistake = _mistakeRepository.GetMistakeById(id);

            if (mistake == null)
                return NotFound();

            var mistakeVM = new MistakeViewModel()
            {
                MistakeId = mistake.MistakeId,
                Description = mistake.Description,
                ManualType = mistake.ManualType,
                DateRegister = mistake.DateRegister,
                ImageUrl = mistake.ImageUrl,
                Name = mistake.Name,
                Status = mistake.Status
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
