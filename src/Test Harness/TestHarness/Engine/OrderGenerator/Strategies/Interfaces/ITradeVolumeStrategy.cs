using System.Collections.Generic;
using DomainV2.Equity.Frames;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityTick> frames);
    }
}