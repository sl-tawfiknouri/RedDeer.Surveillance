using System;
using DataImport.Recorders.Interfaces;
using DomainV2.Equity.Frames;

namespace DataImport.Recorders
{
    // ReSharper disable once UnusedMember.Global
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
