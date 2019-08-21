namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    using System;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Timebars;

    using MathNet.Numerics.Distributions;

    using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;

    /// <summary>
    ///     A strategy to update security ticks by sampling the next value
    ///     from the standard deviation
    /// </summary>
    public class MarkovEquityStrategy : IEquityDataGeneratorStrategy
    {
        private readonly decimal _maxSpread = 0.001m;

        private readonly double _pricingStandardDeviation = 0.1; // good value for 15 minute tick updates

        private readonly double _tradingStandardDeviation = 4; // volume of trades will track larger volatility

        public MarkovEquityStrategy()
        {
        }

        /// <summary>
        ///     Constructor for overriding default rule values
        /// </summary>
        /// <param name="pricingStandardDeviation">standard deviation to sample prices</param>
        /// <param name="tradingStandardDeviation">standard deviation to sample trades</param>
        /// <param name="maxSpread">Number [0..0.99]</param>
        public MarkovEquityStrategy(double pricingStandardDeviation, double tradingStandardDeviation, decimal maxSpread)
        {
            this._pricingStandardDeviation = pricingStandardDeviation;
            this._tradingStandardDeviation = tradingStandardDeviation;
            this._maxSpread = maxSpread;

            if (maxSpread >= 1 || maxSpread < 0) throw new ArgumentOutOfRangeException(nameof(maxSpread));
        }

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Markov;

        public EquityInstrumentIntraDayTimeBar AdvanceFrame(
            EquityInstrumentIntraDayTimeBar tick,
            DateTime advanceTick,
            bool walkIntraday)
        {
            if (tick == null) return null;

            var newBuy = this.CalculateNewBuyValue(tick);
            var newSell = this.CalculateNewSellValue(tick, newBuy);
            var newVolume = this.CalculateNewVolume(tick);

            var newSpread = new SpreadTimeBar(
                new Money(newBuy, tick.SpreadTimeBar.Bid.Currency),
                new Money(newSell, tick.SpreadTimeBar.Ask.Currency),
                new Money(newBuy, tick.SpreadTimeBar.Bid.Currency),
                newVolume);

            var newMarketCap =
                (tick.DailySummaryTimeBar.ListedSecurities ?? tick.DailySummaryTimeBar.DailyVolume.Traded) * newBuy;

            var newIntraday = walkIntraday
                                  ? this.BuildIntraday(tick, newBuy, tick.SpreadTimeBar.Bid.Currency.Code)
                                  : tick.DailySummaryTimeBar.IntradayPrices ?? this.BuildIntraday(
                                        tick,
                                        newBuy,
                                        tick.SpreadTimeBar.Bid.Currency.Code);

            var newDaily = new DailySummaryTimeBar(
                newMarketCap,
                newIntraday,
                tick.DailySummaryTimeBar.ListedSecurities,
                tick.DailySummaryTimeBar.DailyVolume,
                advanceTick);

            return new EquityInstrumentIntraDayTimeBar(tick.Security, newSpread, newDaily, advanceTick, tick.Market);
        }

        private IntradayPrices BuildIntraday(EquityInstrumentIntraDayTimeBar tick, decimal newBuy, string currency)
        {
            if (tick.DailySummaryTimeBar.IntradayPrices?.High == null
                || tick.DailySummaryTimeBar.IntradayPrices?.Low == null)
                return new IntradayPrices(
                    new Money(newBuy, currency),
                    new Money(newBuy, currency),
                    new Money(newBuy, currency),
                    new Money(newBuy, currency));

            var adjustedHigh = tick.DailySummaryTimeBar.IntradayPrices.High.Value.Value < newBuy
                                   ? new Money(newBuy, tick.DailySummaryTimeBar.IntradayPrices.High.Value.Currency)
                                   : tick.DailySummaryTimeBar.IntradayPrices.High.Value;

            var adjustedLow = tick.DailySummaryTimeBar.IntradayPrices.Low.Value.Value < newBuy
                                  ? new Money(newBuy, tick.DailySummaryTimeBar.IntradayPrices.High.Value.Currency)
                                  : tick.DailySummaryTimeBar.IntradayPrices.Low.Value;

            var newIntraday = new IntradayPrices(
                tick.DailySummaryTimeBar.IntradayPrices.Open,
                tick.DailySummaryTimeBar.IntradayPrices.Close,
                adjustedHigh,
                adjustedLow);

            return newIntraday;
        }

        private decimal CalculateNewBuyValue(EquityInstrumentIntraDayTimeBar tick)
        {
            var newBuy = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Price.Value, this._pricingStandardDeviation);

            if (newBuy < 0.01m) newBuy = 0.01m;

            newBuy = Math.Round(newBuy, 2);

            return newBuy;
        }

        private decimal CalculateNewSellValue(EquityInstrumentIntraDayTimeBar tick, decimal newBuy)
        {
            var newSellSample = (decimal)Normal.Sample(
                (double)tick.SpreadTimeBar.Price.Value,
                this._pricingStandardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = newBuy * (1 - this._maxSpread); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;
            newSell = Math.Round(newSell, 2);

            return newSell;
        }

        private Volume CalculateNewVolume(EquityInstrumentIntraDayTimeBar tick)
        {
            var newVolumeSample = (int)Normal.Sample(tick.SpreadTimeBar.Volume.Traded, this._tradingStandardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

            return newVolume;
        }
    }
}