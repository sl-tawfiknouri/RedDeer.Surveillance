using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    public interface IOrderBrokerRepository
    {
        Task UpdateEnrichedBroker(IReadOnlyCollection<BrokerEnrichmentDto> brokers);
        Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers();
        Task<string> InsertOrUpdateBroker(IOrderBroker broker);
    }
}