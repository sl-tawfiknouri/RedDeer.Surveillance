using System;
using DomainV2.Equity.Frames;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<ExchangeFrame>
    {
    }
}
