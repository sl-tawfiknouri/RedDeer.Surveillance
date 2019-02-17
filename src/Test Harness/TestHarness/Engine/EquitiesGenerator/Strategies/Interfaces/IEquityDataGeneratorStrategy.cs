using System;
using Domain.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        EquityInstrumentIntraDayTimeBar AdvanceFrame(EquityInstrumentIntraDayTimeBar tick, DateTime advanceTick, bool walkIntraday);
        EquityGenerationStrategies Strategy { get; }
    }
}