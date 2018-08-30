using Domain.Equity.Trading.Frames;
using System;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<ExchangeFrame>
    {
    }
}
