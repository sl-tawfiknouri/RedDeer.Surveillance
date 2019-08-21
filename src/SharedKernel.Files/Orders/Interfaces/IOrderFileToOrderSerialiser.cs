namespace SharedKernel.Files.Orders.Interfaces
{
    using Domain.Core.Trading.Orders;

    public interface IOrderFileToOrderSerialiser
    {
        int FailedParseTotal { get; set; }

        OrderFileContract[] Map(Order order);

        Order Map(OrderFileContract contract);
    }
}