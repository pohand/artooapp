using Artoo.IRepositories;
using Artoo.Models;
using Artoo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFactoryRepository _factoryRepository;
        private readonly ITechManagerRepository _techManagerRepository;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IFactoryRepository factoryRepository,
            ITechManagerRepository techManagerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _factoryRepository = factoryRepository;
            _techManagerRepository = techManagerRepository;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                        return RedirectToAction("Index", "Home");

                    return Redirect(loginViewModel.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Username/password not found");
            return View(loginViewModel);
        }


        public IActionResult Register()
        {
            LoginViewModel model = new LoginViewModel();
            model.Roles = GetSelectListRoles();
            model.Factories = GetSelectListFactories();
            model.TechManagers = GetSelectListTechManagers();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel loginViewModel)
        {
            if (loginViewModel.RoleId == "0")
            {
                ModelState.AddModelError("RoleId", "Please select role");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = loginViewModel.UserName,
                    Email = loginViewModel.Email,
                    FactoryId = loginViewModel.FactoryId == 0 ? null : loginViewModel.FactoryId
                };
                var result = await _userManager.CreateAsync(user, loginViewModel.Password);

                if (result.Succeeded)
                {
                    IdentityRole applicationRole = await _roleManager.FindByIdAsync(loginViewModel.RoleId);
                    if (applicationRole != null)
                    {
                        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, applicationRole.Name);
                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            AddErrors(roleResult);
                        }
                    }

                    ViewBag.message = "Success";
                }
                else
                {
                    AddErrors(result);
                }
            }

            loginViewModel.Roles = GetSelectListRoles();
            loginViewModel.Factories = GetSelectListFactories();
            loginViewModel.TechManagers = GetSelectListTechManagers();

            return View(loginViewModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index()
        {
            List<LoginViewModel> model = new List<LoginViewModel>();
            model = _userManager.Users.Select(u => new LoginViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                //RoleName = _roleManager.Roles.Single(r => r.Name == _userManager.GetRolesAsync(u).Result.Single()).Name
            }).ToList();

            foreach (var item in model)
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(item.Id);
                var role = _userManager.GetRolesAsync(applicationUser).Result.SingleOrDefault();
                if (role != null)
                {
                    item.RoleName = _roleManager.Roles.Single(r => r.Name == role).Name;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser applicationUser = await _userManager.FindByIdAsync(id);
                if (applicationUser != null)
                {
                    IdentityResult result = await _userManager.DeleteAsync(applicationUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }        
        public async Task<IActionResult> Details(string id)
        {
            LoginViewModel model = new LoginViewModel();
            model.Roles = GetSelectListRoles();
            model.Factories = GetSelectListFactories();
            model.TechManagers = GetSelectListTechManagers();

            if (!String.IsNullOrEmpty(id))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    model.Id = user.Id;
                    model.UserName = user.UserName;
                    model.Email = user.Email;
                    model.RoleId = _roleManager.Roles.Single(r => r.Name == _userManager.GetRolesAsync(user).Result.Single()).Id;
                    model.FactoryId = user.FactoryId;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Details(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.FactoryId = model.FactoryId == 0 ? null : model.FactoryId;
                    string existingRole = _userManager.GetRolesAsync(user).Result.Single();
                    string existingRoleId = _roleManager.Roles.Single(r => r.Name == existingRole).Id;
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        if (existingRoleId != model.RoleId)
                        {
                            IdentityResult roleResult = await _userManager.RemoveFromRoleAsync(user, existingRole);
                            if (roleResult.Succeeded)
                            {
                                IdentityRole applicationRole = await _roleManager.FindByIdAsync(model.RoleId);
                                if (applicationRole != null)
                                {
                                    IdentityResult newRoleResult = await _userManager.AddToRoleAsync(user, applicationRole.Name);
                                    if (newRoleResult.Succeeded)
                                    {
                                        ViewBag.message = "Success";
                                    }
                                }
                            }
                        }
                        ViewBag.message = "Success";
                    }
                }
            }
            else
            {
                model.Roles = GetSelectListRoles();
                model.Factories = GetSelectListFactories();
                model.TechManagers = GetSelectListTechManagers();
            }
            return View(model);
        }

        public List<SelectListItem> GetSelectListRoles()
        {
            return _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
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

        public List<SelectListItem> GetSelectListTechManagers()
        {
            return _techManagerRepository.TechManagers.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.TechManagerId.ToString()
            }).ToList();
        }
       
    }
}
