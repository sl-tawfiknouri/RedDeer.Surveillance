using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Rules.WashTrade
{
    /// <summary>
    /// This is a naive pairing algorithm
    /// We have plans to use clustering instead in the future
    /// </summary>
    public class WashTradePositionPairer : IWashTradePositionPairer
    {
        public IReadOnlyCollection<PositionCluster> PairUp(
            List<Order> trades,
            IWashTradeRuleParameters parameters)
        {
            if (trades == null
                || !trades.Any()
                || parameters == null)
            {
                return new PositionCluster[0];
            }

            trades = trades.Where(tr => tr != null).ToList();

            var positionPairs = new List<PositionCluster>();
            var currentBuyPosition = new TradePosition(new List<Order>());
            var currentSellPosition = new TradePosition(new List<Order>());
            var benchmarkPrice = 0m;

            foreach (var trade in trades)
            {
                if (trade?.OrderAveragePrice == null)
                {
                    continue;
                }

                if (benchmarkPrice == 0)
                {
                    benchmarkPrice = trade.OrderAveragePrice.GetValueOrDefault(0);                   
                }

                if (!InRangeOfCurrentPrice(benchmarkPrice, trade.OrderAveragePrice.GetValueOrDefault(0), parameters))
                {
                    benchmarkPrice = trade.OrderAveragePrice.GetValueOrDefault(0);

                    if (currentBuyPosition.Get().Any()
                        && currentSellPosition.Get().Any())
                    {
                        positionPairs.Add(new PositionCluster(currentBuyPosition, currentSellPosition));
                    }

                    currentBuyPosition = new TradePosition(new List<Order>());
                    currentSellPosition = new TradePosition(new List<Order>());
                }

                if (trade.OrderPosition == OrderPositions.BUY)
                {
                    currentBuyPosition.Add(trade);
                }

                if (trade.OrderPosition == OrderPositions.SELL)
                {
                    currentSellPosition.Add(trade);
                }
            }

            if (currentBuyPosition.Get().Any()
                && currentSellPosition.Get().Any())
            {
                positionPairs.Add(new PositionCluster(currentBuyPosition, currentSellPosition));
            }

            return positionPairs;
        }

        private bool InRangeOfCurrentPrice(decimal currentPrice, decimal newPrice, IWashTradeRuleParameters parameters)
        {
            if (currentPrice == 0
                || newPrice == 0)
            {
                return false;
            }

            var offset = currentPrice * parameters.PairingPositionPercentagePriceChangeThresholdPerPair.GetValueOrDefault(0);

            var lowerBoundary = currentPrice - offset;
            var upperBoundary = currentPrice + offset;

            if (lowerBoundary <= newPrice
                && upperBoundary >= newPrice)
            {
                return true;
            }

            return false;
        }
    }
}
