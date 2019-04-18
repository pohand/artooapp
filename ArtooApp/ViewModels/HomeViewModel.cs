using Artoo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class EmployeeModel
    {
        public int EmpId { get; set; }
        [Display(Name = "Employee Name")]
        //[DataType(DataType.MultilineText)]
        public string EmpName { get; set; }
    }
}
