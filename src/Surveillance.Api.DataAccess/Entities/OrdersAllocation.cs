namespace Surveillance.Api.DataAccess.Entities
{
    using System;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrdersAllocation : IOrdersAllocation
    {
        public bool AutoScheduled { get; set; }

        public string ClientAccountId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Fund { get; set; }

        public int Id { get; set; }

        public bool Live { get; set; }

        public long OrderFilledVolume { get; set; }

        public string OrderId { get; set; }

        public string Strategy { get; set; }
    }
}