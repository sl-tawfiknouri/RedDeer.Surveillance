using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Markets;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace Surveillance.DataLayer.Aurora.BMLL
{
    public class StubRuleRunDataRequestRepository : IStubRuleRunDataRequestRepository
    {
        public async Task CreateDataRequest(MarketDataRequest request)
        {
        }

        public async Task<bool> HasDataRequestForRuleRun(string ruleRunId)
        {
            return false;
        }

        public async Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForRuleRun(string ruleRunId)
        {
            return new MarketDataRequest[0];
        }

        public async Task UpdateToComplete(IReadOnlyCollection<MarketDataRequest> requests)
        {
        }
    }
}
