using System;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IEquityDataGenerator
    {
        void InitiateWalk(IStockExchangeStream stream, TimeSpan tickFrequency);
        void TerminateWalk();
    }
}