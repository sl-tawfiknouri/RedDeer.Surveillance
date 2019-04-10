using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class OrderLedger : IOrderLedger
    {
        public string Name { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;

        public IOrder[] Orders { get; set; } = new Order[0];

        public static OrderLedger Null()
        {
            return new OrderLedger();
        }
    }
}
