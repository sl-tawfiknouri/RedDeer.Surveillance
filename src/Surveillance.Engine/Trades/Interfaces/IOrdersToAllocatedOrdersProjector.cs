using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface IOrdersToAllocatedOrdersProjector
    {
        Task<IReadOnlyCollection<Order>> DecorateOrders(IReadOnlyCollection<Order> orders);
    }
}