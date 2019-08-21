namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class OrderLedger : IOrderLedger
    {
        public string Manager { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public IOrder[] Orders { get; set; } = new Order[0];

        public static OrderLedger Null()
        {
            return new OrderLedger();
        }
    }
}