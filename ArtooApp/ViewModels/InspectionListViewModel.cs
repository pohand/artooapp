using Artoo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class InspectionListViewModel
    {
        public List<SelectListItem> FinalWeekList { get; set; }
        public string FinalWeekSearchString { get; set; }
        public List<SelectListItem> FactoryList { get; set; }
        public string FactorySearchString { get; set; }
        public string OrderNumberSearchString { get; set; }
        public string TechManagerNameSearchString { get; set; }
        public IEnumerable<InspectionViewModel> InspectionList { get; set; }
        public List<SelectListItem> ResultList { get; set; }
        public string ResultSearchString { get; set; }
        public PageViewModel Page { get; set; }
    }
}
