﻿using System.Collections.Generic;
using Domain.Equity.Frames;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class TradeVolumeFixedStrategy : ITradeVolumeStrategy
    {
        private readonly int _fixedVolume;

        public TradeVolumeFixedStrategy(int fixedVolume)
        {
            _fixedVolume = fixedVolume;
        }

        public int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityTick> frames)
        {
            if (frames == null)
            {
                return 0;
            }

            return _fixedVolume;
        }
    }
}