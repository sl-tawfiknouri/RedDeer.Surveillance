using System.Collections.Generic;
using System.Linq;
using Domain.Financial;
using Domain.Trading;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    /// <summary>
    /// This is a naive pairing algorithm
    /// We have plans to use clustering instead in the future
    /// </summary>
    public class WashTradePositionPairer : IWashTradePositionPairer
    {
        public IReadOnlyCollection<PositionCluster> PairUp(
            List<Order> trades,
            IWashTradeRuleEquitiesParameters equitiesParameters)
        {
            if (trades == null
                || !trades.Any()
                || equitiesParameters == null)
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
                if (trade?.OrderAverageFillPrice == null)
                {
                    continue;
                }

                if (benchmarkPrice == 0)
                {
                    benchmarkPrice = trade.OrderAverageFillPrice.GetValueOrDefault().Value;                   
                }

                if (!InRangeOfCurrentPrice(benchmarkPrice, trade.OrderAverageFillPrice.GetValueOrDefault().Value, equitiesParameters))
                {
                    benchmarkPrice = trade.OrderAverageFillPrice.GetValueOrDefault().Value;

                    if (currentBuyPosition.Get().Any()
                        && currentSellPosition.Get().Any())
                    {
                        positionPairs.Add(new PositionCluster(currentBuyPosition, currentSellPosition));
                    }

                    currentBuyPosition = new TradePosition(new List<Order>());
                    currentSellPosition = new TradePosition(new List<Order>());
                }

                if (trade.OrderDirection == OrderDirections.BUY 
                    || trade.OrderDirection == OrderDirections.COVER)
                {
                    currentBuyPosition.Add(trade);
                }

                if (trade.OrderDirection == OrderDirections.SELL
                    || trade.OrderDirection == OrderDirections.SHORT)
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

        private bool InRangeOfCurrentPrice(decimal currentPrice, decimal newPrice, IWashTradeRuleEquitiesParameters equitiesParameters)
        {
            if (currentPrice == 0
                || newPrice == 0)
            {
                return false;
            }

            var offset = currentPrice * equitiesParameters.PairingPositionPercentagePriceChangeThresholdPerPair.GetValueOrDefault(0);

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
