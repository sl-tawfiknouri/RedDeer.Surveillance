namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Markets.Timebars;

    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames);
    }
}