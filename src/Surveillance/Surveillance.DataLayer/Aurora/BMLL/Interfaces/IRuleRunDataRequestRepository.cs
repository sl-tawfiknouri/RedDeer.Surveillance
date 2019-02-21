using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Markets;

namespace Surveillance.DataLayer.Aurora.BMLL.Interfaces
{
    public interface IRuleRunDataRequestRepository
    {
        Task CreateDataRequest(MarketDataRequest request);
        Task<bool> HasDataRequestForRuleRun(string ruleRunId);
        Task<IReadOnlyCollection<MarketDataRequest>> DataRequestsForSystemOperation(string systemOperationId);
        Task UpdateToCompleteWithDuplicates(IReadOnlyCollection<MarketDataRequest> requests);
    }
}