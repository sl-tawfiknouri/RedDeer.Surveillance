namespace SharedKernel.Files.Orders.Interfaces
{
    using Domain.Core.Trading.Orders;

    public interface IOmsOrderFieldCompression
    {
        Order Compress(Order orderToCompress, Order orderToRetain);
    }
}