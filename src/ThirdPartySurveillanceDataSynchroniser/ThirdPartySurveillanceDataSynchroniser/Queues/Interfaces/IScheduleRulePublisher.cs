using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Queues.Interfaces
{
    public interface IScheduleRulePublisher
    {
        Task RescheduleRuleRun(string systemProcessOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests);
    }
}