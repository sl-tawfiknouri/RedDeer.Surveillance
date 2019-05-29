using System;
using Domain.Core.Trading.Orders;

namespace TestHarness.Engine.OrderStorage.Interfaces
{
    public interface IOrderFileStorageProcess : IObserver<Order>
    {
    }
}
