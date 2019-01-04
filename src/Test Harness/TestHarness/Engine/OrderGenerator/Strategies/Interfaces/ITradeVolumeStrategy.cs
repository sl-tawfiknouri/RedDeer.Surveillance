using System.Collections.Generic;
using DomainV2.Equity.TimeBars;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<FinancialInstrumentTimeBar> frames);
    }
}