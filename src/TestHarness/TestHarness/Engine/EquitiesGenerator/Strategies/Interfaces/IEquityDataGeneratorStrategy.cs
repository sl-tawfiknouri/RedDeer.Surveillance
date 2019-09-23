namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    using System;

    using Domain.Core.Markets.Timebars;

    public interface IEquityDataGeneratorStrategy
    {
        EquityGenerationStrategies Strategy { get; }

        EquityInstrumentIntraDayTimeBar AdvanceFrame(
            EquityInstrumentIntraDayTimeBar tick,
            DateTime advanceTick,
            bool walkIntraday);
    }
}