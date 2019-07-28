using Artoo.Models;
using System.Collections.Generic;

namespace Artoo.ViewModels
{
    public class MistakeListViewModel
    {
        public IEnumerable<Mistake> ManualMistakes { get; set; }
        public PageViewModel ManualMistakePage { get; set; }
        public IEnumerable<Mistake> DeviceMistakes { get; set; }
        public PageViewModel DeviceMistakePage { get; set; }
    }
}
