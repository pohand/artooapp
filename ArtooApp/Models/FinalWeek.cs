using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class FinalWeek
    {
        public int FinalWeekId { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public int? Week { get; set; }
        public int? Year { get; set; }
        public DateTime? FinalWeekDay { get; set; }
        public DateTime DateRegister { get; set; }
    }
}
