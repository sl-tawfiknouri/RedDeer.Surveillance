using System;
using Domain.Equity.Frames;

namespace DataImport.Network_IO.RelaySubscribers.Interfaces
{
    public interface IEquityRelaySubscriber : IObserver<ExchangeFrame>
    {
        bool Initiate(string domain, string port);
        void Terminate();
    }
}