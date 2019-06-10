using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class MistakeFree : BaseEntity
    {
        public int MistakeFreeId { get; set; }
        public string Description { get; set; }
        [Display(Name = "Lỗi")]
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime DateRegister { get; set; }
        public int InspectionId { get; set; }
        public virtual Inspection Inspection { get; set; }
    }
}
