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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    public class EmailRuleController : Controller
    {
        private readonly IEmailRuleRepository _emailRuleRepository;
        private readonly IPassionBrandRepository _passionBrandRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IConfigurationRepository _configurationRepository;

        public EmailRuleController(IEmailRuleRepository emailRuleRepository,
            IPassionBrandRepository passionBrandRepository,
            IEmailRepository emailRepository,
            IConfigurationRepository configurationRepository)
        {
            _emailRuleRepository = emailRuleRepository;
            _passionBrandRepository = passionBrandRepository;
            _emailRepository = emailRepository;
            _configurationRepository = configurationRepository;
        }

        public List<SelectListItem> PassionBrands()
        {
            return _passionBrandRepository.PassionBrands
               .Select(r => new SelectListItem
               {
                   Text = r.Name,
                   Value = r.PassionBrandId.ToString()
               }).ToList();
        }

        public List<SelectListItem> Emails()
        {
            return _emailRepository.Emails
                .Select(x => new SelectListItem
                {
                    Text = x.EmailAddress,
                    Value = x.EmailId.ToString()
                }).ToList();
        }

        public List<SelectListItem> GetEnumList(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            var results = new List<SelectListItem>();

            foreach (var i in values)
            {
                results.Add(new SelectListItem
                {
                    Text = Enum.GetName(enumType, i),
                    Value = ((int)i).ToString()
                });
            }
            return results;
        }

        public IActionResult Create()
        {
            var passionBrands = PassionBrands();
            var emails = Emails();
            var results = GetEnumList(typeof(InspectionResultEnum));
            var orderTypes = GetEnumList(typeof(OrderTypeEnum));
            var emailIds = new List<int>();
            EmailRuleViewModel emailRuleVM = new EmailRuleViewModel()
            {
                PassionBrands = passionBrands,
                Emails = emails,
                ResultList = results,
                EmailIds = emailIds,
                OrderTypeList = orderTypes
            };

            return View(emailRuleVM);
        }

        [HttpPost]
        public IActionResult Create(EmailRuleViewModel emailRuleVM)
        {
            if (ModelState.IsValid)
            {
                var emailRule = new EmailRule()
                {
                    PassionBrandId = emailRuleVM.PassionBrandId,
                    Result = emailRuleVM.Result,
                    OrderType = emailRuleVM.OrderType
                };

                var emailRuleDetailId = _emailRuleRepository.CreateEmailRule(emailRule);

                List<EmailRuleDetail> erds = new List<EmailRuleDetail>();
                foreach (var item in emailRuleVM.EmailIds)
                {
                    var erd = new EmailRuleDetail()
                    {
                        EmailId = item,
                        EmailRuleId = emailRuleDetailId
                    };

                    erds.Add(erd);
                }

                _emailRuleRepository.CreateEmailRuleDetail(erds);
                ViewBag.message = "Success";
            }
            return RedirectToAction("Index");
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            IEnumerable<EmailRule> emailRules = _emailRuleRepository.GetEmailRuleIncludePassion();

            EmailRuleListViewModel emailRuleListVM = new EmailRuleListViewModel();
            List<EmailRuleViewModel> emailRulesVM = new List<EmailRuleViewModel>();

            foreach (var item in emailRules)
            {
                IEnumerable<EmailRuleDetail> emailRuleDetail = _emailRuleRepository.GetEmailRuleDetailByEmailRuleId(item.EmailRuleId);
                List<int> emailIds = emailRuleDetail.Select(x => x.EmailId).ToList();
                IEnumerable<Email> emails = _emailRepository.GetEmailByIdList(emailIds);

                EmailRuleViewModel emailRuleVM = new EmailRuleViewModel()
                {
                    EmailRuleId = item.EmailRuleId,
                    Result = item.Result,
                    OrderType = item.OrderType,
                    EmailList = emails,
                    PassionBrand = item.PassionBrand
                };

                emailRulesVM.Add(emailRuleVM);
            }

            ArtooConfiguration config = _configurationRepository.GetConfigurationByName(ConfigurationEnum.sendAllMail.ToString());
            bool status = config == null ? false : config.Status;

            return View(new EmailRuleListViewModel
            {
                EmailRules = emailRulesVM,
                SendAllMail = status
            });
        }

        [HttpPost]
        public IActionResult Index(EmailRuleListViewModel emailRuleListVM)
        {
            ArtooConfiguration configuration = new ArtooConfiguration{
                Name = ConfigurationEnum.sendAllMail.ToString(),
                Status = emailRuleListVM.SendAllMail
            };

            _configurationRepository.UpdateCofigurationByName(configuration);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Details(EmailRuleViewModel emailRuleVM)
        {
            if (ModelState.IsValid)
            {
                var emailRule = new EmailRule()
                {
                    EmailRuleId = emailRuleVM.EmailRuleId,
                    PassionBrandId = emailRuleVM.PassionBrandId,
                    Result = emailRuleVM.Result,
                    OrderType = emailRuleVM.OrderType
                };
                _emailRuleRepository.UpdateEmailRule(emailRule);

                _emailRuleRepository.DeleteEmailRuleDetailByEmailRuleId(emailRuleVM.EmailRuleId);
                List<EmailRuleDetail> erds = new List<EmailRuleDetail>();
                foreach (var item in emailRuleVM.EmailIds)
                {
                    var erd = new EmailRuleDetail()
                    {
                        EmailId = item,
                        EmailRuleId = emailRuleVM.EmailRuleId
                    };

                    erds.Add(erd);
                }

                _emailRuleRepository.CreateEmailRuleDetail(erds);

                ViewBag.message = "Success";
            }

            var passionBrands = PassionBrands();
            var emails = Emails();
            var results = GetEnumList(typeof(InspectionResultEnum));
            var orderTypes = GetEnumList(typeof(OrderTypeEnum));

            emailRuleVM.PassionBrands = passionBrands;
            emailRuleVM.Emails = emails;
            emailRuleVM.ResultList = results;
            emailRuleVM.OrderTypeList = orderTypes;
            //return View(emailRuleVM);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var existing = _emailRuleRepository.GetEmailRuleById(id);
            if (existing == null)
                return NotFound();

            var passionBrands = PassionBrands();
            var emails = Emails();
            var results = GetEnumList(typeof(InspectionResultEnum));
            var orderTypes = GetEnumList(typeof(OrderTypeEnum));

            var emailRuleDetail = _emailRuleRepository.GetEmailRuleDetailByEmailRuleId(existing.EmailRuleId);
            var emailRuleVM = new EmailRuleViewModel()
            {
                EmailRuleId = existing.EmailRuleId,
                PassionBrandId = existing.PassionBrandId,
                Result = existing.Result,
                OrderType = existing.OrderType,
                Emails = emails,
                PassionBrands = passionBrands,
                ResultList = results,
                OrderTypeList = orderTypes,
                EmailIds = emailRuleDetail.Select(x=>x.EmailId).ToList()
            };

            return View(emailRuleVM);
        }

        public IActionResult Delete(int id)
        {
            var email = _emailRuleRepository.GetEmailRuleById(id);
            if (email == null)
                return NotFound();

            _emailRuleRepository.DeleteEmailRule(id);
            return RedirectToAction("Index");
        }
    }
}
