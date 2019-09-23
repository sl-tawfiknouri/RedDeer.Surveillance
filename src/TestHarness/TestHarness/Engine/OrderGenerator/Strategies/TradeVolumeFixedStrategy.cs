namespace TestHarness.Engine.OrderGenerator.Strategies
{
    using System.Collections.Generic;

    using Domain.Core.Markets.Timebars;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    public class TradeVolumeFixedStrategy : ITradeVolumeStrategy
    {
        private readonly int _fixedVolume;

        public TradeVolumeFixedStrategy(int fixedVolume)
        {
            this._fixedVolume = fixedVolume;
        }

        public int CalculateSecuritiesToTrade(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> frames)
        {
            if (frames == null) return 0;

            return this._fixedVolume;
        }
    }
}