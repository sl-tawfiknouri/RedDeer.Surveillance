using System.Collections.Generic;
using Domain.Equity.Trading.Frames;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class TradeVolumeFixedStrategy : ITradeVolumeStrategy
    {
        private int _fixedVolume;

        public TradeVolumeFixedStrategy(int fixedVolume)
        {
            _fixedVolume = fixedVolume;
        }

        public int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityFrame> frames)
        {
            if (frames == null)
            {
                return 0;
            }

            return _fixedVolume;
        }
    }
}