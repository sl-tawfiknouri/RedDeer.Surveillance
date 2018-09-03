using System.Collections.Generic;
using Domain.Equity.Frames;

namespace TestHarness.Engine.OrderGenerator.Strategies.Interfaces
{
    public interface ITradeVolumeStrategy
    {
        int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityFrame> frames);
    }
}