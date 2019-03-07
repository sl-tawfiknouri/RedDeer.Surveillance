using MathNet.Numerics.Distributions;
using System;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using Domain.Core.Financial;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Timebars;

namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    /// <summary>
    /// A strategy to update security ticks by sampling the next value
    /// from the standard deviation
    /// </summary>
    public class MarkovEquityStrategy : IEquityDataGeneratorStrategy
    {
        private readonly double _pricingStandardDeviation = 0.1; // good value for 15 minute tick updates
        private readonly double _tradingStandardDeviation = 4; // volume of trades will track larger volatility
        private readonly decimal _maxSpread = 0.001m;

        public MarkovEquityStrategy()
        { }

        /// <summary>
        /// Constructor for overriding default rule values
        /// </summary>
        /// <param name="pricingStandardDeviation">standard deviation to sample prices</param>
        /// <param name="tradingStandardDeviation">standard deviation to sample trades</param>
        /// <param name="maxSpread">Number [0..0.99]</param>
        public MarkovEquityStrategy(double pricingStandardDeviation, double tradingStandardDeviation, decimal maxSpread)
        {
            _pricingStandardDeviation = pricingStandardDeviation;
            _tradingStandardDeviation = tradingStandardDeviation;
            _maxSpread = maxSpread;

            if (maxSpread >= 1 || maxSpread < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSpread));
            }
        }

        public EquityInstrumentIntraDayTimeBar AdvanceFrame(EquityInstrumentIntraDayTimeBar tick, DateTime advanceTick, bool walkIntraday)
        {
            if (tick == null)
            {
                return null;
            }

            var newBuy = CalculateNewBuyValue(tick);
            var newSell = CalculateNewSellValue(tick, newBuy);
            var newVolume = CalculateNewVolume(tick);

            var newSpread =
                new SpreadTimeBar(
                    new Money(newBuy, tick.SpreadTimeBar.Bid.Currency),
                    new Money(newSell, tick.SpreadTimeBar.Ask.Currency),
                    new Money(newBuy, tick.SpreadTimeBar.Bid.Currency),
                    newVolume);

            
            var newMarketCap = (tick.DailySummaryTimeBar.ListedSecurities ?? tick.DailySummaryTimeBar.DailyVolume.Traded) * newBuy;

            var newIntraday =
                walkIntraday
                    ? BuildIntraday(tick, newBuy, tick.SpreadTimeBar.Bid.Currency.Code)
                    : (tick.DailySummaryTimeBar.IntradayPrices ?? BuildIntraday(tick, newBuy, tick.SpreadTimeBar.Bid.Currency.Code));

            var newDaily = new DailySummaryTimeBar(
                newMarketCap,
                newIntraday,
                tick.DailySummaryTimeBar.ListedSecurities,
                tick.DailySummaryTimeBar.DailyVolume,
                advanceTick);

            return
                new EquityInstrumentIntraDayTimeBar(
                    tick.Security,
                    newSpread,
                   newDaily,
                    advanceTick,
                    tick.Market);
        }

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Markov;

        private IntradayPrices BuildIntraday(EquityInstrumentIntraDayTimeBar tick, decimal newBuy, string currency)
        {
            if (tick.DailySummaryTimeBar.IntradayPrices?.High == null
                || tick.DailySummaryTimeBar.IntradayPrices?.Low == null)
            {
                return
                    new IntradayPrices(
                        new Money(newBuy, currency),
                        new Money(newBuy, currency),
                        new Money(newBuy, currency),
                        new Money(newBuy, currency));
            }

            var adjustedHigh =
                tick.DailySummaryTimeBar.IntradayPrices.High.Value.Value < newBuy
                ? new Money(newBuy, tick.DailySummaryTimeBar.IntradayPrices.High.Value.Currency)
                : tick.DailySummaryTimeBar.IntradayPrices.High.Value;

            var adjustedLow =
                tick.DailySummaryTimeBar.IntradayPrices.Low.Value.Value < newBuy
                ? new Money(newBuy, tick.DailySummaryTimeBar.IntradayPrices.High.Value.Currency)
                : tick.DailySummaryTimeBar.IntradayPrices.Low.Value;

            var newIntraday =
                new IntradayPrices(
                    tick.DailySummaryTimeBar.IntradayPrices.Open,
                    tick.DailySummaryTimeBar.IntradayPrices.Close,
                    adjustedHigh,
                    adjustedLow);

            return newIntraday;
        }

        private decimal CalculateNewBuyValue(EquityInstrumentIntraDayTimeBar tick)
        {
            var newBuy = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Price.Value, _pricingStandardDeviation);

            if (newBuy < 0.01m)
            {
                newBuy = 0.01m;
            }

            newBuy = Math.Round(newBuy, 2);

            return newBuy;
        }

        private decimal CalculateNewSellValue(EquityInstrumentIntraDayTimeBar tick, decimal newBuy)
        {
            var newSellSample = (decimal)Normal.Sample((double)tick.SpreadTimeBar.Price.Value, _pricingStandardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = (newBuy * (1 - _maxSpread)); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;
            newSell = Math.Round(newSell, 2);

            return newSell;
        }

        private Volume CalculateNewVolume(EquityInstrumentIntraDayTimeBar tick)
        {
            var newVolumeSample = (int)Normal.Sample(tick.SpreadTimeBar.Volume.Traded, _tradingStandardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

            return newVolume;
        }
    }
}
