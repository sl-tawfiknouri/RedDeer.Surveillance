using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace SharedKernel.Files.Orders.Interfaces
{
    public interface IOmsVersioner
    {
        IReadOnlyCollection<Order> ProjectOmsVersion(IReadOnlyCollection<Order> orders);
    }
}