namespace Surveillance.Data.Universe.Trades.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    /// <summary>
    /// The OrdersToAllocatedOrdersProjector interface.
    /// </summary>
    public interface IOrdersToAllocatedOrdersProjector
    {
        /// <summary>
        /// The decorate orders.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IReadOnlyCollection<Order>> DecorateOrders(IReadOnlyCollection<Order> orders);
    }
}