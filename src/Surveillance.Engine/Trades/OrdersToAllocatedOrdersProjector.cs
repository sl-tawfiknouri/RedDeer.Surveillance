using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Trading;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Trades
{
    public class OrdersToAllocatedOrdersProjector : IOrdersToAllocatedOrdersProjector
    {
        private readonly IOrderAllocationRepository _orderAllocationRepository;

        public OrdersToAllocatedOrdersProjector(IOrderAllocationRepository orderAllocationRepository)
        {
            _orderAllocationRepository = orderAllocationRepository ?? throw new ArgumentNullException(nameof(orderAllocationRepository));
        }

        public async Task<IReadOnlyCollection<Order>> DecorateOrders(IReadOnlyCollection<Order> orders)
        {
            orders = orders?.Where(o => o != null)?.ToList();

            if (orders == null
                || !orders.Any())
            {
                return new Order[0];
            }

            var orderIds = orders.Select(o => o.OrderId).ToList();
            var allocations = await _orderAllocationRepository.Get(orderIds);
            var groupedAllocations = allocations.GroupBy(i => i.OrderId).ToDictionary((a) => a.Key, (a) => a.ToList());
            var decoratedOrders = orders.SelectMany(o => Project(o, groupedAllocations)).ToList();

            return decoratedOrders;
        }

        private IReadOnlyCollection<OrderAllocationDecorator> Project(Order order, IDictionary<string, List<OrderAllocation>> groupedAllocations)
        {
            if (!groupedAllocations.ContainsKey(order.OrderId))
            {
                var allocation = new OrderAllocation(order);
                var decorator = new OrderAllocationDecorator(order, allocation);

                return new [] { decorator };
            }

            var allocations = groupedAllocations[order.OrderId];
            var decoratedAllocations =
                allocations
                    ?.Select(alloc => new OrderAllocationDecorator(order, alloc))
                    ?.ToList() 
                ?? new List<OrderAllocationDecorator>();

            return decoratedAllocations;
        }
    }
}
