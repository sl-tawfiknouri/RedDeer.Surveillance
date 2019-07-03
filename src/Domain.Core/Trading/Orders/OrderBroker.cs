using System;
using Domain.Core.Trading.Orders.Interfaces;

namespace Domain.Core.Trading.Orders
{
    public class OrderBroker : IOrderBroker
    {
        public OrderBroker(
            string id,
            string reddeerId,
            string name,
            DateTime? createdOn,
            bool live)
        {
            Id = id ?? string.Empty;
            ReddeerId = reddeerId ?? string.Empty;
            Name = name ?? string.Empty;
            CreatedOn = createdOn;
            Live = live;
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// AKA external id
        /// </summary>
        public string ReddeerId { get; }

        public string Name { get; }

        public DateTime? CreatedOn { get; }

        public bool Live { get; }
    }
}
