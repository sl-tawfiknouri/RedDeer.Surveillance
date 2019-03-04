using Domain.Trading;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IOrderFileToOrderSerialiser
    {
        OrderFileContract[] Map(Order order);
        Order Map(OrderFileContract contract);
        int FailedParseTotal { get; set; }
    }
}