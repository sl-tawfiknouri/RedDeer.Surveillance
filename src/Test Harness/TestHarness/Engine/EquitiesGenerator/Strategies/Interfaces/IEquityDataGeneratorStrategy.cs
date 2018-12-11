using System;
using DomainV2.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityTick AdvanceFrame(SecurityTick tick, DateTime advanceTick, bool walkIntraday);
        EquityGenerationStrategies Strategy { get; }
    }
}