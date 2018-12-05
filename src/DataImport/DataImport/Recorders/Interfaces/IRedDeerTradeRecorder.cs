using System;
using DomainV2.Trading;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerTradeRecorder : IObserver<Order>
    {
    }
}
