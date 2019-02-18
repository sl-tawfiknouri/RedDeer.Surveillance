using System.Collections.Generic;
using Domain.Equity.TimeBars;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames);
    }
}