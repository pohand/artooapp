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
    public class EmailController : Controller
    {
        private readonly IEmailRepository _emailRepository;

        public EmailController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Email email)
        {
            if (ModelState.IsValid)
            {
                _emailRepository.CreateEmail(email);
                return RedirectToAction("Index");
            }
            return View(email);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            IEnumerable<Email> emails = _emailRepository.Emails.OrderBy(p => p.EmailId);

            return View(new EmailListViewModel
            {
                Emails = emails
            });
        }

        [HttpPost]
        public IActionResult Update(Email email)
        {
            if (ModelState.IsValid)
            {
                _emailRepository.UpdateEmail(email);
                return RedirectToAction("Index");
            }

            return View(email);
        }

        public IActionResult Details(int id)
        {
            var email = _emailRepository.GetEmailById(id);
            if (email == null)
                return NotFound();

            return View(email);
        }

        public IActionResult Delete(int id)
        {
            var email = _emailRepository.GetEmailById(id);
            if (email == null)
                return NotFound();

            _emailRepository.DeleteEmail(id);
            return RedirectToAction("Index");
        }
    }
}
