namespace TestHarness.Engine.OrderStorage.Interfaces
{
    using System;

    using Domain.Core.Trading.Orders;

    public interface IOrderFileStorageProcess : IObserver<Order>
    {
    }
}