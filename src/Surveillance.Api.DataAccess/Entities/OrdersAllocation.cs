using System;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class OrdersAllocation : IOrdersAllocation
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string Fund { get; set; }
        public string Strategy { get; set; }
        public string ClientAccountId { get; set; }
        public long OrderFilledVolume { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Live { get; set; }
        public bool Autoscheduled { get; set; }
    }
}
