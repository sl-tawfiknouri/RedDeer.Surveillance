using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces
{
    public interface IBmllDataRequestsRescheduleManager
    {
        Task RescheduleRuleRun(string ruleRunId, List<MarketDataRequestDataSource> bmllRequests);
    }
}