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

        public SecurityFrame AdvanceFrame(SecurityFrame frame)
        {
            if (frame == null)
            {
                return null;
            }

            decimal newBuy = CalculateNewBuyValue(frame);
            decimal newSell = CalculateNewSellValue(frame, newBuy);

            var newSpread = new Spread(new Price(newBuy), new Price(newSell));
            var newVolume = CalculateNewVolume(frame);

            return new SecurityFrame(frame.Security, newSpread, newVolume);
        }

        private decimal CalculateNewBuyValue(SecurityFrame frame)
        {
            var newBuy = (decimal)Normal.Sample((double)frame.Spread.Buy.Value, _pricingStandardDeviation);

            if (newBuy < 0.001m)
            {
                newBuy = 0.001m;
            }

            newBuy = Math.Round(newBuy, 2);

            return newBuy;
        }

        private decimal CalculateNewSellValue(SecurityFrame frame, decimal newBuy)
        {
            var newSellSample = (decimal)Normal.Sample((double)frame.Spread.Sell.Value, _pricingStandardDeviation);

            var newSellLimit = Math.Min(newBuy, newSellSample);
            var newSellFloor = (newBuy * (1 - _maxSpread)); // allow for a max of 5% spread
            var newSell = newSellFloor > newSellLimit ? newSellFloor : newSellLimit;
            newSell = Math.Round(newSell, 2);

            return newSell;
        }

        private Volume CalculateNewVolume(SecurityFrame frame)
        {
            var newVolumeSample = (int)Normal.Sample(frame.Volume.Traded, _tradingStandardDeviation);
            var newVolumeSampleFloor = Math.Max(0, newVolumeSample);
            var newVolume = new Volume(newVolumeSampleFloor);

            return newVolume;
        }
    }
}
