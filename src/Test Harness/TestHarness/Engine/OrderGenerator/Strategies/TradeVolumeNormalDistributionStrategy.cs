﻿using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public class TradeVolumeNormalDistributionStrategy : ITradeVolumeStrategy
    {
        private int _sd;

        public TradeVolumeNormalDistributionStrategy(int sd)
        {
            _sd = sd;
        }

        public int CalculateSecuritiesToTrade(IReadOnlyCollection<SecurityFrame> frames)
        {
            var tradingMean = TradingMean(frames);
            var totalSecuritiesToTrade = (int)Normal.Sample(tradingMean, _sd);
            var adjustedSecuritiesToTrade = Math.Max(totalSecuritiesToTrade, 0);

            return adjustedSecuritiesToTrade;
        }

        private int TradingMean(IReadOnlyCollection<SecurityFrame> frames)
        {
            var rawCount = frames.Count();

            if (rawCount <= 0)
            {
                return 0;
            }

            var sqrt = Math.Sqrt(rawCount);

            if (sqrt < 10)
            {
                return rawCount;
            }

            return (int)sqrt;
        }
    }
}
