using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class TechManager : BaseEntity
    {
        public int TechManagerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime DateRegister { get; set; }
    }
}
