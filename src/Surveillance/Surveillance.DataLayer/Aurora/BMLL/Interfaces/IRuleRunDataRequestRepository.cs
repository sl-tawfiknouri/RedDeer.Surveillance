using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Markets;

namespace Surveillance.DataLayer.Aurora.BMLL.Interfaces
{
    public interface IRuleRunDataRequestRepository
    {
        Task CreateDataRequest(MarketDataRequest request);
        Task<bool> HasDataRequestForRuleRun(string ruleRunId);
        Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForRuleRun(string ruleRunId);
    }
}