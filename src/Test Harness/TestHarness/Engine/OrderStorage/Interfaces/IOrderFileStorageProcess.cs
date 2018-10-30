using System;
using Domain.Trades.Orders;

namespace TestHarness.Engine.OrderStorage.Interfaces
{
    public interface IOrderFileStorageProcess : IObserver<TradeOrderFrame>
    {
    }
}
