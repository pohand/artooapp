using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class DashboardViewModel
    {
        public BlokingRateViewModel BlokingRate { get; set; }
        public BlokingRateListViewModel BlockingRateList { get; set; }

        public DPMViewModel DPM { get; set; }
        public DPMListViewModel DPMList { get; set; }

        public int NumberPONeedToInspect { get; set; }
        public List<NumberOfPONeedCheckViewModel> NumberOfPONeedToCheckList { get; set; }
        public BRChart BlockingRateChart { get; set; }
    }

    public class BlokingRateViewModel
    {
        public int RejectPO { get; set; }
        public int TotalPO { get; set; }
        public string FactoryName { get; set; }
        public int BlockingRate { get; set; }
        public int? FactoryId { get; set; }
    }

    public class BlokingRateListViewModel
    {
        public IEnumerable<BlokingRateViewModel> BlockingRateList { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
    }

    public class NumberOfPONeedCheckViewModel
    {
        public int NumberOfPONeedToCheck { get; set; }
        public int TotalPO { get; set; }
        public string FactoryName { get; set; }
        public int BlockingRate { get; set; }
        public int? FactoryId { get; set; }
    }

    public class DPMViewModel
    {
        public int? MistakeItems { get; set; }
        public int CheckedItems { get; set; }
        public string FactoryName { get; set; }
        public double? DPMValue { get; set; }
        public int? FactoryId { get; set; }
    }

    public class DPMListViewModel
    {
        public IEnumerable<DPMViewModel> DPMList { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
    }

    public class BRChart
    {
        //public List<string> WeekList { get; set; }
        //public List<int> BRValues { get; set; }
        public IEnumerable<BRDatapoint> BRDatapoints { get; set; }
        public int Threshold { get; set; }
        public int Top { get; set; }
    }

    public class BRDatapoint
    {
        public string Week { get; set; }
        public int BRValue { get; set; }
    }
}
