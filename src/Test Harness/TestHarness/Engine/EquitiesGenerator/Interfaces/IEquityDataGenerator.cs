using System;
using Domain.Equity.Trading.Streams.Interfaces;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IEquityDataGenerator
    {
        void InitiateWalk(IStockExchangeStream stream, IHeartbeat heartbeat);
        void TerminateWalk();
    }
}