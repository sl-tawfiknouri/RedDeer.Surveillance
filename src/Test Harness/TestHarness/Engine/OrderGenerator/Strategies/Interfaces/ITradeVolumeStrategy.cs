using System.Collections.Generic;
using Domain.Equity.Trading.Frames;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityFrame> frames);
    }
}