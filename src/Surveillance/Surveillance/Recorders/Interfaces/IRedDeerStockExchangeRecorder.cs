using System;
using DomainV2.Equity.Frames;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<ExchangeFrame>
    {
    }
}
