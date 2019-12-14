namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

    public interface IOrderBrokerRepository
    {
        Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers();

        Task<string> InsertOrUpdateBrokerAsync(IOrderBroker broker);

        Task UpdateEnrichedBroker(IReadOnlyCollection<BrokerEnrichmentDto> brokers);
    }
}