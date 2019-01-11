using System;
using DomainV2.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        FinancialInstrumentTimeBar AdvanceFrame(FinancialInstrumentTimeBar tick, DateTime advanceTick, bool walkIntraday);
        EquityGenerationStrategies Strategy { get; }
    }
}