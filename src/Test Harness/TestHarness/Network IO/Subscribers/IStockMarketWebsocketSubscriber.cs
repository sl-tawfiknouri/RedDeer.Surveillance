﻿using Domain.Equity.Trading.Frames;
using System;

namespace TestHarness.Network_IO.Subscribers
{
    public interface IStockMarketWebsocketSubscriber : IObserver<ExchangeFrame>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}