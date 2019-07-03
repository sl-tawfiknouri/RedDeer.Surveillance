using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    public interface IOrderBrokerRepository
    {
        Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers();
        Task<string> InsertOrUpdateBroker(IOrderBroker broker);
    }
}