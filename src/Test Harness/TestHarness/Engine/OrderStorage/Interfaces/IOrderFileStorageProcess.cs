using System;
using DomainV2.Trading;

namespace TestHarness.Engine.OrderStorage.Interfaces
{
    public interface IOrderFileStorageProcess : IObserver<Order>
    {
    }
}
