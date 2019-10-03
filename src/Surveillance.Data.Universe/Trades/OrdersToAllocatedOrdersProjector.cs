namespace Surveillance.Data.Universe.Trades
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using Surveillance.Data.Universe.Trades.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    /// <summary>
    /// The orders to allocated orders projector.
    /// </summary>
    public class OrdersToAllocatedOrdersProjector : IOrdersToAllocatedOrdersProjector
    {
        /// <summary>
        /// The order allocation repository.
        /// </summary>
        private readonly IOrderAllocationRepository orderAllocationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersToAllocatedOrdersProjector"/> class.
        /// </summary>
        /// <param name="orderAllocationRepository">
        /// The order allocation repository.
        /// </param>
        public OrdersToAllocatedOrdersProjector(IOrderAllocationRepository orderAllocationRepository)
        {
            this.orderAllocationRepository =
                orderAllocationRepository ?? throw new ArgumentNullException(nameof(orderAllocationRepository));
        }

        /// <summary>
        /// The decorate orders.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<Order>> DecorateOrders(IReadOnlyCollection<Order> orders)
        {
            orders = orders?.Where(o => o != null)?.ToList();

            if (orders == null || !orders.Any())
            {
                return new Order[0];
            }

            var orderIds = orders.Select(o => o.OrderId).ToList();
            var allocations = await this.orderAllocationRepository.Get(orderIds);
            var groupedAllocations = allocations.GroupBy(i => i.OrderId).ToDictionary(a => a.Key, a => a.ToList());
            var decoratedOrders = orders.SelectMany(o => this.Project(o, groupedAllocations)).ToList();

            return decoratedOrders;
        }

        /// <summary>
        /// The project.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="groupedAllocations">
        /// The grouped allocations.
        /// </param>
        /// <returns>
        /// The <see cref="OrderAllocationDecorator"/>.
        /// </returns>
        private IReadOnlyCollection<OrderAllocationDecorator> Project(
            Order order,
            IDictionary<string, List<OrderAllocation>> groupedAllocations)
        {
            if (!groupedAllocations.ContainsKey(order.OrderId))
            {
                var allocation = new OrderAllocation(order);
                var decorator = new OrderAllocationDecorator(order, allocation);

                return new[] { decorator };
            }

            var allocations = groupedAllocations[order.OrderId];
            var decoratedAllocations =
                allocations?.Select(alloc => new OrderAllocationDecorator(order, alloc))?.ToList()
                ?? new List<OrderAllocationDecorator>();

            return decoratedAllocations;
        }
    }
}