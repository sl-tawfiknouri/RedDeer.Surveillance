using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Core.Trading.Orders.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    public interface IOrderBrokerRepository
    {
        Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers();
        string InsertOrUpdateBroker(IOrderBroker brokerName);
    }
}