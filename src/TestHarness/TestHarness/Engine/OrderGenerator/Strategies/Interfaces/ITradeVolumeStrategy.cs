using System.Collections.Generic;
using Domain.Core.Markets.Timebars;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames);
    }
}