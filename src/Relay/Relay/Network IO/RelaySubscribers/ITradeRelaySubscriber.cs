using Domain.Equity.Trading.Orders;
using System;

namespace Relay.Network_IO.RelaySubscribers
{
    public interface ITradeRelaySubscriber : IObserver<TradeOrderFrame>
    {
        void Initiate(string domain, string port);
        void Terminate();
    }
}