using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class InspectionMistakeDetail
    {
        public int InspectionMistakeDetailId { get; set; }
        public int InspectionId { get; set; }
        public int MistakeId { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public virtual Mistake Mistake { get; set; }
        public virtual Inspection Inspection { get; set; }
    }
}
