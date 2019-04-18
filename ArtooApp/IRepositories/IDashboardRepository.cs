using Artoo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.IRepositories
{
    public interface IDashboardRepository
    {
        BlokingRateViewModel GetBlockingRate();
        BlokingRateListViewModel GetBlockingRateByFactory();
        DPMListViewModel GetDPMByFactory();
        DPMViewModel GetDPM();
        List<NumberOfPONeedCheckViewModel> GetNumberOfPONeedToCheckListByFactory(BlokingRateListViewModel blockingRateList);
        BRChart GetBRChartDatapoint();
    }
}
