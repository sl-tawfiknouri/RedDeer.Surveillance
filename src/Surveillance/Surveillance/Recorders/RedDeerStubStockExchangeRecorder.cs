using Domain.Equity.Trading.Frames;
using Surveillance.Recorders.Interfaces;
using System;

namespace Surveillance.Recorders
{
    public class RedDeerStubStockExchangeRecorder : IRedDeerStockExchangeRecorder
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ExchangeFrame value)
        {
        }
    }
}
