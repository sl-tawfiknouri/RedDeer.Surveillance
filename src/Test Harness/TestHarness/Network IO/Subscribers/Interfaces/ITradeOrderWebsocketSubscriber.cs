using System;
using DomainV2.Trading;

namespace TestHarness.Network_IO.Subscribers.Interfaces
{
    public interface ITradeOrderWebsocketSubscriber : IObserver<Order>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}