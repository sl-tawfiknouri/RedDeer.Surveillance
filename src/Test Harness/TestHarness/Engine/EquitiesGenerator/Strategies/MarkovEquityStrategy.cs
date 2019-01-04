using MathNet.Numerics.Distributions;
using System;
using DomainV2.Equity;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;

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

        public FinancialInstrumentTimeBar AdvanceFrame(FinancialInstrumentTimeBar tick, DateTime advanceTick, bool walkIntraday)
        {
            if (tick == null)
            {
                return null;
            }

            var newBuy = CalculateNewBuyValue(tick);
            var newSell = CalculateNewSellValue(tick, newBuy);

            var newSpread =
                new Spread(
                    new CurrencyAmount(newBuy, tick.Spread.Bid.Currency),
                    new CurrencyAmount(newSell, tick.Spread.Ask.Currency),
                    new CurrencyAmount(newBuy, tick.Spread.Bid.Currency));
            var newVolume = CalculateNewVolume(tick);
            var newMarketCap = (tick.ListedSecurities ?? tick.DailyVolume.Traded) * newBuy;

            var newIntraday =
                walkIntraday
                    ? BuildIntraday(tick, newBuy, tick.Spread.Bid.Currency.Value)
                    : (tick.IntradayPrices ?? BuildIntraday(tick, newBuy, tick.Spread.Bid.Currency.Value));

            return
                new FinancialInstrumentTimeBar(
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

        public EquityGenerationStrategies Strategy { get; } = EquityGenerationStrategies.Markov;

        private IntradayPrices BuildIntraday(FinancialInstrumentTimeBar tick, decimal newBuy, string currency)
        {
            if (tick.IntradayPrices?.High == null
                || tick.IntradayPrices?.Low == null)
            {
                return
                    new IntradayPrices(
                        new CurrencyAmount(newBuy, currency),
                        new CurrencyAmount(newBuy, currency),
                        new CurrencyAmount(newBuy, currency),
                        new CurrencyAmount(newBuy, currency));
            }

            var adjustedHigh =
                tick.IntradayPrices.High.Value.Value < newBuy
                ? new CurrencyAmount(newBuy, tick.IntradayPrices.High.Value.Currency)
                : tick.IntradayPrices.High.Value;

            var adjustedLow =
                tick.IntradayPrices.Low.Value.Value < newBuy
                ? new CurrencyAmount(newBuy, tick.IntradayPrices.High.Value.Currency)
                : tick.IntradayPrices.Low.Value;

            var newIntraday =
                new IntradayPrices(
                    tick.IntradayPrices.Open,
                    tick.IntradayPrices.Close,
                    adjustedHigh,
                    adjustedLow);

            return newIntraday;
        }

        private decimal CalculateNewBuyValue(FinancialInstrumentTimeBar tick)
        {
            var newBuy = (decimal)Normal.Sample((double)tick.Spread.Price.Value, _pricingStandardDeviation);

            if (newBuy < 0.001m)
            {
                newBuy = 0.001m;
            }

            newBuy = Math.Round(newBuy, 2);

            return newBuy;
        }

        private decimal CalculateNewSellValue(FinancialInstrumentTimeBar tick, decimal newBuy)
        {
            var newSellSample = (decimal)Normal.Sample((double)tick.Spread.Price.Value, _pricingStandardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = (newBuy * (1 - _maxSpread)); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;
            newSell = Math.Round(newSell, 2);

            return newSell;
        }

        private Volume CalculateNewVolume(FinancialInstrumentTimeBar tick)
        {
            var newVolumeSample = (int)Normal.Sample(tick.Volume.Traded, _tradingStandardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

            return newVolume;
        }
    }
}
