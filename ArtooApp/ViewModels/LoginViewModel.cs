using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class LoginViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public List<SelectListItem> Roles { get; set; }
        [Display(Name = "Role")]
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public List<SelectListItem> Factories { get; set; }
        [Display(Name = "Factory")]
        public int? FactoryId { get; set; }
        public List<SelectListItem> TechManagers { get; set; }
        [Display(Name = "Kĩ thuật trưởng")]
        public int? TechManagerId { get; set; }
    }
}
