using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class EmailRule : BaseEntity
    {
        public int EmailRuleId { get; set; }

        [Required(ErrorMessage = "Xin vui lòng chọn Passion Barnd")]
        public int PassionBrandId { get; set; }

        public virtual PassionBrand PassionBrand { get; set; }

        [Required(ErrorMessage = "Xin vui lòng chọn Accpect/Reject")]
        public int Result { get; set; }
        //public int? EmailId { get; set; }
        //public virtual Email Email { get; set; }
    }
}
