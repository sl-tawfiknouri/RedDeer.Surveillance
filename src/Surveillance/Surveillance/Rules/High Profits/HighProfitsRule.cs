using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Market;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.High_Profits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : BaseUniverseRule, IHighProfitRule
    {
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly IHighProfitMessageSender _sender;
        private readonly IHighProfitsRuleParameters _parameters;

        public HighProfitsRule(
            IHighProfitMessageSender sender,
            IHighProfitsRuleParameters parameters,
            ILogger<HighProfitsRule> logger) 
            : base(
                parameters.WindowSize,
                Domain.Scheduling.Rules.HighProfits,
                "V1.0",
                "High Profit Rule",
                logger)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at =>
                    at.OrderStatus == OrderStatus.PartialFulfilled
                    || at.OrderStatus == OrderStatus.Fulfilled)
                .ToList();

            var cost = CalculateCostOfPosition(liveTrades);
            var revenue = CalculateRevenueOfPosition(liveTrades);

            if (revenue <= 0
                || cost <= 0)
            {
                return;
            }

            var absoluteProfit = revenue - cost;
            var profitRatio = (revenue / cost) - 1;

            if (HasHighProfitAbsolute(absoluteProfit)
                && HasHighProfitPercentage(profitRatio))
            {
                // raise alert
                var breach =
                    new HighProfitRuleBreach(
                        _parameters,
                        absoluteProfit,
                        _parameters.HighProfitAbsoluteThresholdCurrency,
                        profitRatio,
                        null,
                        true,
                        true,
                        activeTrades);

                _sender.Send(breach);
            }
            else if (HasHighProfitPercentage(profitRatio))
            {
                // raise alert
                var breach =
                    new HighProfitRuleBreach(
                        _parameters,
                        absoluteProfit,
                        _parameters.HighProfitAbsoluteThresholdCurrency,
                        profitRatio,
                        null,
                        true,
                        false,
                        activeTrades);

                _sender.Send(breach);
            }
            else if (HasHighProfitAbsolute(absoluteProfit))
            {
                // raise alert
                var breach =
                    new HighProfitRuleBreach(
                        _parameters,
                        absoluteProfit,
                        _parameters.HighProfitAbsoluteThresholdCurrency,
                        profitRatio,
                        null,
                        false,
                        true,
                        activeTrades);

                _sender.Send(breach);
            }
        }

        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return _parameters.HighProfitPercentageThreshold.HasValue
               && _parameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        private bool HasHighProfitAbsolute(decimal profit)
        {
            return _parameters.HighProfitAbsoluteThreshold.HasValue
               && _parameters.HighProfitAbsoluteThreshold.Value <= profit;
        }

        /// <summary>
        /// Sum the total buy in for the position
        /// </summary>
        private decimal CalculateCostOfPosition(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return 0;
            }

            var purchaseOrders =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.BuyLong)
                    .Select(afto => afto.Volume * afto.ExecutedPrice)
                    .Sum();

            return purchaseOrders.GetValueOrDefault(0);
        }

        /// <summary>
        /// Take realised profits and then for any remaining amount use virtual profits
        /// </summary>
        private decimal CalculateRevenueOfPosition(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return 0;
            }
            // TODO take into account carry over positions from the last day as well
            // we might have revenues that are nuts; were only interested in intraday positions
            var realisedRevenue =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.SellLong)
                    .Select(afto => afto.Volume * afto.ExecutedPrice)
                    .Sum();

            var totalPurchaseVolume =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.BuyLong)
                    .Select(afto => afto.Volume)
                    .Sum();

            var totalSaleVolume =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.SellLong)
                    .Select(afto => afto.Volume)
                    .Sum();

            var sizeOfVirtualPosition = totalSaleVolume - totalPurchaseVolume;
            if (sizeOfVirtualPosition <= 0)
            {
                return realisedRevenue.GetValueOrDefault(0);
            }

            var security = activeFulfilledTradeOrders?.FirstOrDefault()?.Security;
            if (security == null)
            {
                return realisedRevenue.GetValueOrDefault(0);
            }

            var securityTick =
                LatestExchangeFrame
                    .Securities
                    .FirstOrDefault(sec => Equals(sec.Security.Identifiers, security.Identifiers));

            var virtualRevenue = securityTick?.Spread.Price.Value * sizeOfVirtualPosition;

            return realisedRevenue.GetValueOrDefault(0) + virtualRevenue.GetValueOrDefault(0);
        }

        protected override void TradingOpen(StockExchange exchange)
        {
            _logger.LogDebug($"Trading Opened for exchange {exchange.Name} in the High Profit Rule");
        }

        protected override void TradingClose(StockExchange exchange)
        {
            // TODO check closing profits for all positions && raise alerts if necessary
            _logger.LogDebug($"Trading closed for exchange {exchange.Name} in the High Profit Rule");
        }

        protected override void Genesis()
        {
            _logger.LogDebug("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void EndOfUniverse()
        {
            // TODO check all profits one last time IF trading has opened without being closed
            _logger.LogDebug("Universe Eschaton occurred in the High Profit Rule");
        }
    }
}
