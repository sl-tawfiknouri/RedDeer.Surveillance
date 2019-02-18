using System;
using Domain.Trading;

namespace TestHarness.Engine.OrderStorage.Interfaces
{
    public interface IOrderFileStorageProcess : IObserver<Order>
    {
    }
}
