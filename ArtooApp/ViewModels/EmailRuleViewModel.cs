using Artoo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class EmailRuleViewModel
    {
        public int EmailRuleId { get; set; }

        [Required(ErrorMessage = "Xin vui lòng chọn Passion Barnd")]
        public int PassionBrandId { get; set; }

        public List<SelectListItem> PassionBrands { get; set; }

        [Required(ErrorMessage = "Xin vui lòng chọn Accpect/Reject")]
        public int Result { get; set; }
        public List<SelectListItem> ResultList { get; set; }
        public int EmailId { get; set; }
        public List<SelectListItem> Emails { get; set; }
        public List<int> EmailIds { get; set; }
        public IEnumerable<Email> EmailList { get; set; }
        public PassionBrand PassionBrand { get; set; }
    }
}
