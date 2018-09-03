using System;
using Domain.Equity.Frames;

namespace TestHarness.Network_IO.Subscribers.Interfaces
{
    public interface IStockMarketWebsocketSubscriber : IObserver<ExchangeFrame>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}