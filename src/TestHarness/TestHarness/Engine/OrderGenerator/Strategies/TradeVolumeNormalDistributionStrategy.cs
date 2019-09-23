namespace TestHarness.Engine.OrderGenerator.Strategies
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Markets.Timebars;

    using MathNet.Numerics.Distributions;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    public class TradeVolumeNormalDistributionStrategy : ITradeVolumeStrategy
    {
        private readonly int _sd;

        public TradeVolumeNormalDistributionStrategy(int sd)
        {
            this._sd = sd;
        }

        public int CalculateSecuritiesToTrade(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames)
        {
            var tradingMean = this.TradingMean(frames);
            var totalSecuritiesToTrade = (int)Normal.Sample(tradingMean, this._sd);
            var adjustedSecuritiesToTrade = Math.Max(totalSecuritiesToTrade, 0);

            return adjustedSecuritiesToTrade;
        }

        private int TradingMean(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames)
        {
            var rawCount = frames.Count;

            if (rawCount <= 0) return 0;

            var sqrt = Math.Sqrt(rawCount);

            if (sqrt < 10) return rawCount;

            return (int)sqrt;
        }
    }
}