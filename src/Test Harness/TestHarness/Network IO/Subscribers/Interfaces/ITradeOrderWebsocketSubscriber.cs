using Domain.Equity.Trading.Orders;
using System;

namespace TestHarness.Network_IO.Subscribers.Interfaces
{
    public interface ITradeOrderWebsocketSubscriber : IObserver<TradeOrderFrame>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}