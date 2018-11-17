using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
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
            List<TradeOrderFrame> trades,
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
            var currentBuyPosition = new TradePosition(new List<TradeOrderFrame>());
            var currentSellPosition = new TradePosition(new List<TradeOrderFrame>());
            var benchmarkPrice = 0m;

            foreach (var trade in trades)
            {
                if (trade?.ExecutedPrice == null)
                {
                    continue;
                }

                if (benchmarkPrice == 0)
                {
                    benchmarkPrice = trade.ExecutedPrice.Value.Value;                   
                }

                if (!InRangeOfCurrentPrice(benchmarkPrice, trade.ExecutedPrice.Value.Value, parameters))
                {
                    benchmarkPrice = trade.ExecutedPrice.Value.Value;

                    if (currentBuyPosition.Get().Any()
                        && currentSellPosition.Get().Any())
                    {
                        positionPairs.Add(new PositionCluster(currentBuyPosition, currentSellPosition));
                    }

                    currentBuyPosition = new TradePosition(new List<TradeOrderFrame>());
                    currentSellPosition = new TradePosition(new List<TradeOrderFrame>());
                }

                if (trade.Position == OrderPosition.Buy)
                {
                    currentBuyPosition.Add(trade);
                }

                if (trade.Position == OrderPosition.Sell)
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

            if (((currentPrice - offset) <= newPrice)
                && ((currentPrice + offset) >= newPrice))
            {
                return true;
            }

            return false;
        }
    }
}
