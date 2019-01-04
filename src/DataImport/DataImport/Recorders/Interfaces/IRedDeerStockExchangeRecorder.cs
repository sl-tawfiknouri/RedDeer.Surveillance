using System;
using DomainV2.Equity.TimeBars;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<MarketTimeBarCollection>
    {
    }
}
