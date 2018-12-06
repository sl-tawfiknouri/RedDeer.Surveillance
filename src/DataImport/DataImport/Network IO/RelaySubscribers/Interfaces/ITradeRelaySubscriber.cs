using System;
using DomainV2.Trading;

namespace DataImport.Network_IO.RelaySubscribers.Interfaces
{
    public interface ITradeRelaySubscriber : IObserver<Order>
    {
        /// <summary>
        /// Indicates success of operation
        /// </summary>
        bool Initiate(string domain, string port);
        void Terminate();
    }
}