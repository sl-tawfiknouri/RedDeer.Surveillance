using System;
using Domain.Equity.Frames;

namespace DataImport.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<ExchangeFrame>
    {
    }
}
