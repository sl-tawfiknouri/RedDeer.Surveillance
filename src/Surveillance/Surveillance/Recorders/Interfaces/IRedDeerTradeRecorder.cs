using System;
using DomainV2.Trading;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerTradeRecorder : IObserver<Order>
    {
    }
}
