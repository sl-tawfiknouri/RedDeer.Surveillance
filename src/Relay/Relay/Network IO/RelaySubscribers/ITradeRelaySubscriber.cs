using Domain.Equity.Trading.Orders;
using System;

namespace Relay.Network_IO.RelaySubscribers
{
    public interface ITradeRelaySubscriber : IObserver<TradeOrderFrame>
    {
        /// <summary>
        /// Indicates success of operation
        /// </summary>
        bool Initiate(string domain, string port);
        void Terminate();
    }
}