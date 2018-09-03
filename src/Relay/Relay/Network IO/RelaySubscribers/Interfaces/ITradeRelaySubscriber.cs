﻿using System;
using Domain.Trades.Orders;

namespace Relay.Network_IO.RelaySubscribers.Interfaces
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