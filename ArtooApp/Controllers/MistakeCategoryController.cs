using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Artoo.Models;
using System.Collections.Generic;
using Artoo.IRepositories;
using Artoo.ViewModels;
using Artoo.Common;

namespace Artoo.Controllers
{
    public class MistakeCategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMistakeCategoryRepository _mistakeCategoryRepository;

        public MistakeCategoryController(AppDbContext context, IMistakeCategoryRepository mistakeCategoryRepository)
        {
            _context = context;
            _mistakeCategoryRepository = mistakeCategoryRepository;
        }

        // GET: MistakeCategory
        public IActionResult Index()
        {
            IEnumerable<MistakeCategory> manualMistakeCategories = _mistakeCategoryRepository.MistakeCategories.Where(x=>x.MistakeType == (int)MistakeEnum.ManualMistake).OrderBy(x => x.Name);
            IEnumerable<MistakeCategory> deviceMistakeCategories = _mistakeCategoryRepository.MistakeCategories.Where(x => x.MistakeType == (int)MistakeEnum.DeviceMistake).OrderBy(x => x.Name);

            ViewBag.Current = "MistakeCategoryList";
            return View(new MistakeCategoryListViewModel
            {
                ManualMistakeCategories = manualMistakeCategories,
                DeviceMistakeCategories = deviceMistakeCategories
            });
        }

        // GET: MistakeCategory/Create
        public IActionResult Create(string type)
        {
            var model = new MistakeCategory();
            if (type == "1")
            {
                model.MistakeType = 1;
            }

            ViewBag.Current = "MistakeCategoryList";
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(MistakeCategory mistakeCategory)
        {
            if (ModelState.IsValid)
            {
                _mistakeCategoryRepository.CreateMistakeCategory(mistakeCategory);
                ViewBag.message = "Success";
                return RedirectToAction("Index");
            }

            ViewBag.Current = "MistakeCategoryList";
            return View(mistakeCategory);
        }

        public IActionResult Details(int id)
        {
            var mistakeCategory = _mistakeCategoryRepository.GetMistakeCategoryById(id);
            if (mistakeCategory == null)
                return NotFound();

            ViewBag.Current = "MistakeCategoryList";
            return View(mistakeCategory);
        }

        public IActionResult Delete(int id)
        {
            var email = _mistakeCategoryRepository.GetMistakeCategoryById(id);
            if (email == null)
                return NotFound();

            _mistakeCategoryRepository.DeleteMistakeCategory(id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Update(MistakeCategory mistakeCategory)
        {
            if (ModelState.IsValid)
            {
                _mistakeCategoryRepository.UpdateMistakeCategory(mistakeCategory);
                ViewBag.message = "Success";
                return RedirectToAction("Index");
            }

            ViewBag.Current = "MistakeCategoryList";
            return View(mistakeCategory);
        }
    }
}
