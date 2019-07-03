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
        public string Id { get; set; }

        /// <summary>
        /// AKA external id
        /// </summary>
        public string ReddeerId { get; set; }

        public string Name { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool Live { get; set; }
    }
}
