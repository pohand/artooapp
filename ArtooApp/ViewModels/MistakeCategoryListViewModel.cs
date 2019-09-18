using Artoo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class MistakeCategoryListViewModel
    {
        public IEnumerable<MistakeCategory> ManualMistakeCategories { get; set; }
        public IEnumerable<MistakeCategory> DeviceMistakeCategories { get; set; }
    }
}
