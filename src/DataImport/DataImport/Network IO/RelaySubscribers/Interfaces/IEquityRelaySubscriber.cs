using System;
using Domain.Equity.Frames;

namespace Relay.Network_IO.RelaySubscribers.Interfaces
{
    public interface IEquityRelaySubscriber : IObserver<ExchangeFrame>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}