namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    public interface IOrdersToAllocatedOrdersProjector
    {
        Task<IReadOnlyCollection<Order>> DecorateOrders(IReadOnlyCollection<Order> orders);
    }
}