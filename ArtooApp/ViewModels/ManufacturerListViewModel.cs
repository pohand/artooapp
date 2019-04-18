using Artoo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class ManufacturerListViewModel
    {
        public IEnumerable<Factory> Manufacturers { get; set; }
    }
}
