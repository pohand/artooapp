using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Artoo.IRepositories;
using Artoo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    public class PassionBrandController : Controller
    {
        private readonly IPassionBrandRepository _passionBrandRepository;

        public PassionBrandController(IPassionBrandRepository passionBrandRepository)
        {
            _passionBrandRepository = passionBrandRepository;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PassionBrand passionBrand)
        {
            if (ModelState.IsValid)
            {
                _passionBrandRepository.CreatePassionBrand(passionBrand);
                return RedirectToAction("Index");
            }
            return View(passionBrand);
        }

        [HttpPost]
        public IActionResult Update(PassionBrand passionBrand)
        {
            if (ModelState.IsValid)
            {
                _passionBrandRepository.UpdatePassionBrand(passionBrand);
                return RedirectToAction("Index");
            }

            return View(passionBrand);
        }

        public ViewResult Index()
        {
            IEnumerable<PassionBrand> passionBrands = _passionBrandRepository.PassionBrands.OrderBy(p => p.PassionBrandId);

            return View(new PassionBrandListViewModel
            {
                PassionBrands = passionBrands
            });
        }

        public IActionResult Details(int id)
        {
            var passionBrand = _passionBrandRepository.GetPassionBrandById(id);
            if (passionBrand == null)
                return NotFound();

            return View(passionBrand);
        }

        public IActionResult Delete(int id)
        {
            var passionBrand = _passionBrandRepository.GetPassionBrandById(id);
            if (passionBrand == null)
                return NotFound();

            _passionBrandRepository.DeletePassionBrand(id);
            return RedirectToAction("Index");
        }
    }
}
