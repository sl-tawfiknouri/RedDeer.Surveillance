namespace SharedKernel.Files.Orders.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public interface IOmsVersioner
    {
        IReadOnlyCollection<Order> ProjectOmsVersion(IReadOnlyCollection<Order> orders);
    }
}