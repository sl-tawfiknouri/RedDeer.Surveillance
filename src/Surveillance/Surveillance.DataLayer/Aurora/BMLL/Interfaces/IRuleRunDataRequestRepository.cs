namespace Surveillance.DataLayer.Aurora.BMLL.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    public interface IRuleRunDataRequestRepository
    {
        Task CreateDataRequest(MarketDataRequest request);

        Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForSystemOperation(string systemOperationId);

        Task<bool> HasDataRequestForRuleRun(string ruleRunId);

        Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests);
    }
}