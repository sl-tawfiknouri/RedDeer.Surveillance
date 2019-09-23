namespace SharedKernel.Files.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Orders.Interfaces;

    /// <summary>
    ///     Receive a list of (OMS) versioned files
    ///     Consolidate records and return combined state
    ///     Supports monotonic integer versioning mechanism
    /// </summary>
    public class OmsVersioner : IOmsVersioner
    {
        private readonly IOmsOrderFieldCompression _omsOrderFieldCompression;

        public OmsVersioner(IOmsOrderFieldCompression omsOrderFieldCompression)
        {
            this._omsOrderFieldCompression = omsOrderFieldCompression
                                             ?? throw new ArgumentNullException(nameof(omsOrderFieldCompression));
        }

        public IReadOnlyCollection<Order> ProjectOmsVersion(IReadOnlyCollection<Order> orders)
        {
            if (orders == null || !orders.Any()) return new Order[0];

            var orderVersions = orders.GroupBy(i => i.OrderVersionLinkId).SelectMany(this.OmsVersionLinkCompression)
                .ToList();

            return orderVersions;
        }

        private IEnumerable<Order> OmsVersionLinkCompression(IEnumerable<Order> orders)
        {
            if (orders == null || !orders.Any()) return new Order[0];

            orders = orders.Where(i => i != null);

            if (orders.Count() <= 1) return orders;

            if (orders.Any(y => string.IsNullOrWhiteSpace(y.OrderVersionLinkId))) return orders;

            var keyedOrders = orders.Select(
                i =>
                    {
                        var orderVersion = 0;
                        int.TryParse(i.OrderVersion, out orderVersion);

                        return new OrderWithKey(orderVersion, i);
                    }).OrderBy(i => i.Key).ToList();

            if (!keyedOrders.Any()) return orders;

            var compressedOrder = keyedOrders.Aggregate(
                (x, y) => new OrderWithKey(y.Key, this._omsOrderFieldCompression.Compress(x.Order, y.Order)));

            return new[] { compressedOrder?.Order };
        }

        private class OrderWithKey
        {
            public OrderWithKey(int key, Order order)
            {
                this.Key = key;
                this.Order = order;
            }

            public int Key { get; }

            public Order Order { get; }
        }
    }
}