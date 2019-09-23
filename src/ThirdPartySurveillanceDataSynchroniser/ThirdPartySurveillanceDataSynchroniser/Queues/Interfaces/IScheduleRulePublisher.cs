namespace DataSynchroniser.Queues.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using SharedKernel.Contracts.Markets;

    public interface IScheduleRulePublisher
    {
        Task RescheduleRuleRun(string systemProcessOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests);
    }
}