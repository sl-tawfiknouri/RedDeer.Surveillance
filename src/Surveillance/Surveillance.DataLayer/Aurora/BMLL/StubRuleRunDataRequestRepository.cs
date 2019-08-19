namespace Surveillance.DataLayer.Aurora.BMLL
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

    public class StubRuleRunDataRequestRepository : IStubRuleRunDataRequestRepository
    {
        public Task CreateDataRequest(MarketDataRequest request)
        {
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForSystemOperation(
            string systemOperationId)
        {
            return await Task.FromResult(new MarketDataRequest[0]);
        }

        public Task<bool> HasDataRequestForRuleRun(string ruleRunId)
        {
            return Task.FromResult(false);
        }

        public Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests)
        {
            return Task.CompletedTask;
        }
    }
}