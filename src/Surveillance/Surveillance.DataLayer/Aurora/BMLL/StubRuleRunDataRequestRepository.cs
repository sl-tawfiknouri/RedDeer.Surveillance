using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace Surveillance.DataLayer.Aurora.BMLL
{
    public class StubRuleRunDataRequestRepository : IStubRuleRunDataRequestRepository
    {
        public Task CreateDataRequest(MarketDataRequest request)
        {
            return Task.CompletedTask;
        }

        public Task<bool> HasDataRequestForRuleRun(string ruleRunId)
        {
            return Task.FromResult(false);
        }

        public async Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForSystemOperation(string systemOperationId)
        {
            return new MarketDataRequest[0];
        }

        public Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests)
        {
            return Task.CompletedTask;
        }
    }
}
