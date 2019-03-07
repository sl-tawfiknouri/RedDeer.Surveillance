using Domain.Core.Trading.Orders;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IOrderFileToOrderSerialiser
    {
        OrderFileContract[] Map(Order order);
        Order Map(OrderFileContract contract);
        int FailedParseTotal { get; set; }
    }
}