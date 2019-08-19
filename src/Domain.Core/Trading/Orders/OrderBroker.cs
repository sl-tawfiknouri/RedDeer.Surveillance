namespace Domain.Core.Trading.Orders
{
    using System;

    using Domain.Core.Trading.Orders.Interfaces;

    public class OrderBroker : IOrderBroker
    {
        public OrderBroker(string id, string reddeerId, string name, DateTime? createdOn, bool live)
        {
            this.Id = id ?? string.Empty;
            this.ReddeerId = reddeerId ?? string.Empty;
            this.Name = name ?? string.Empty;
            this.CreatedOn = createdOn;
            this.Live = live;
        }

        public DateTime? CreatedOn { get; set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public string Id { get; set; }

        public bool Live { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///     AKA external id
        /// </summary>
        public string ReddeerId { get; set; }
    }
}