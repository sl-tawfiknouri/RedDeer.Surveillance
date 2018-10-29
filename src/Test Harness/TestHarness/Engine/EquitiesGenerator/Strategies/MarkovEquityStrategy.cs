using MathNet.Numerics.Distributions;
using System;
using Domain.Equity;
using Domain.Equity.Frames;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    /// <summary>
    /// A strategy to update security ticks by sampling the next value
    /// from the standard deviation
    /// </summary>
    public class MarkovEquityStrategy : IEquityDataGeneratorStrategy
    {
        private readonly double _pricingStandardDeviation = 1; // good value for 15 minute tick updates
        private readonly double _tradingStandardDeviation = 4; // volume of trades will track larger volatility
        private readonly decimal _maxSpread = 0.05m;

        public MarkovEquityStrategy()
        {
        }

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

        public SecurityTick AdvanceFrame(SecurityTick tick, DateTime advanceTick)
        {
            if (tick == null)
            {
                return null;
            }

            var newBuy = CalculateNewBuyValue(tick);
            var newSell = CalculateNewSellValue(tick, newBuy);

            var newSpread =
                new Spread(
                    new Price(newBuy, tick.Spread.Bid.Currency),
                    new Price(newSell, tick.Spread.Ask.Currency),
                    new Price(newBuy, tick.Spread.Bid.Currency));
            var newVolume = CalculateNewVolume(tick);
            var newMarketCap = newVolume.Traded * newBuy;

            var newIntraday = BuildIntraday(tick, newBuy, tick.Spread.Bid.Currency);

            return
                new SecurityTick(
                    tick.Security,
                    newSpread,
                    newVolume,
                    tick.DailyVolume,
                    advanceTick,
                    newMarketCap,
                    newIntraday,
                    tick.ListedSecurities,
                    tick.Market);
        }

        private IntradayPrices BuildIntraday(SecurityTick tick, decimal newBuy, string currency)
        {
            if (tick.IntradayPrices?.High == null
                || tick.IntradayPrices?.Low == null)
            {
                return
                    new IntradayPrices(
                        new Price(newBuy, currency),
                        new Price(newBuy, currency),
                        new Price(newBuy, currency),
                        new Price(newBuy, currency));
            }

            var adjustedHigh =
                tick.IntradayPrices.High.Value.Value < newBuy
                ? new Price(newBuy, tick.IntradayPrices.High.Value.Currency)
                : tick.IntradayPrices.High.Value;

            var adjustedLow =
                tick.IntradayPrices.Low.Value.Value < newBuy
                ? new Price(newBuy, tick.IntradayPrices.High.Value.Currency)
                : tick.IntradayPrices.Low.Value;

            var newIntraday =
                new IntradayPrices(
                    tick.IntradayPrices.Open,
                    tick.IntradayPrices.Close,
                    adjustedHigh,
                    adjustedLow);

            return newIntraday;
        }

        private decimal CalculateNewBuyValue(SecurityTick tick)
        {
            var newBuy = (decimal)Normal.Sample((double)tick.Spread.Bid.Value, _pricingStandardDeviation);

            if (newBuy < 0.001m)
            {
                newBuy = 0.001m;
            }

            newBuy = Math.Round(newBuy, 2);

            return newBuy;
        }

        private decimal CalculateNewSellValue(SecurityTick tick, decimal newBuy)
        {
            var newSellSample = (decimal)Normal.Sample((double)tick.Spread.Ask.Value, _pricingStandardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = (newBuy * (1 - _maxSpread)); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;
            newSell = Math.Round(newSell, 2);

            return newSell;
        }

        private Volume CalculateNewVolume(SecurityTick tick)
        {
            var newVolumeSample = (int)Normal.Sample(tick.Volume.Traded, _tradingStandardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

            return newVolume;
        }
    }
}
