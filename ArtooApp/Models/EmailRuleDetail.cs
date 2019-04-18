using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class EmailRuleDetail : BaseEntity
    {
        public int EmailRuleDetailId { get; set; }
        public int EmailId { get; set; }
        public int EmailRuleId { get; set; }
        public string Description { get; set; }
        public virtual Email Email { get; set; }
        public virtual EmailRule EmailRule { get; set; }
    }
}
