using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class Factory : BaseEntity
    {
        public int FactoryId { get; set; }

        [Required(ErrorMessage = "Xin vui lòng nhập tên xưởng")]
        [Display(Name = "Tên xưởng")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime DateRegister { get; set; }
    }
}
