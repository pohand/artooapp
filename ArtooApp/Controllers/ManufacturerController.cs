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
    public class ManufacturerController : Controller
    {
        private readonly IFactoryRepository _manufacturerRepository;
        public ManufacturerController(IFactoryRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }
        public IActionResult Index()
        {
            IEnumerable<Factory> factories = _manufacturerRepository.Factories.OrderBy(p => p.FactoryId);

            return View(new ManufacturerListViewModel
            {
                Manufacturers = factories
            });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Factory manufacturer)
        {
            if (ModelState.IsValid)
            {
                _manufacturerRepository.CreateFactory(manufacturer);
                return RedirectToAction("Index");
            }
            return View(manufacturer);
        }

        [HttpPost]
        public IActionResult Update(Factory manufacturer)
        {
            if (ModelState.IsValid)
            {
                _manufacturerRepository.UpdateFactory(manufacturer );
                return RedirectToAction("Index");
            }
            return View(manufacturer);
        }

        public IActionResult Details(int id)
        {
            var manufacturer = _manufacturerRepository.GetFactoryById(id);
            if (manufacturer == null)
                return NotFound();

            return View(manufacturer);
        }

        public IActionResult Delete(int id)
        {
            var manufacturer = _manufacturerRepository.GetFactoryById(id);
            if (manufacturer == null)
                return NotFound();

            _manufacturerRepository.DeleteFactory(id);
            return RedirectToAction("Index");
        }
    }
}
