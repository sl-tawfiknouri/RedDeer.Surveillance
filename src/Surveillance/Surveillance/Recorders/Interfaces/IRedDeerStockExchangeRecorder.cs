using System;
using Domain.Equity.Frames;

namespace Surveillance.Recorders.Interfaces
{
    public interface IRedDeerStockExchangeRecorder : IObserver<ExchangeFrame>
    {
    }
}
