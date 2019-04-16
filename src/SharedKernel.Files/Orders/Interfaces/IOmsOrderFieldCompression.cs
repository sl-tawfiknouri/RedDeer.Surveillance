using Domain.Core.Trading.Orders;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IOmsOrderFieldCompression
    {
        Order Compress(Order orderToCompress, Order orderToRetain);
    }
}