﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Market;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Trades.Interfaces;
// ReSharper disable AssignNullToNotNullAttribute

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

        private bool _marketOpened = true; // assume the market has opened initially

        public HighProfitsRule(
            IHighProfitMessageSender sender,
            IHighProfitsRuleParameters parameters,
            ILogger<HighProfitsRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromHours(8),
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

            var hasHighProfitAbsolute = HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = HasHighProfitPercentage(profitRatio);

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                var security = activeTrades.FirstOrDefault(at => at.Security != null)?.Security;

                _logger.LogDebug($"High Profits Rule breach detected for {security?.Identifiers}. Writing breach to message sender.");

                var breach =
                    new HighProfitRuleBreach(
                        _parameters,
                        absoluteProfit,
                        _parameters.HighProfitAbsoluteThresholdCurrency,
                        profitRatio,
                        security,
                        hasHighProfitAbsolute,
                        hasHighProfitPercentage,
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

            var realisedRevenue = CalculateRealisedRevenue(activeFulfilledTradeOrders);
            var totalPurchaseVolume = CalculateTotalPurchaseVolume(activeFulfilledTradeOrders);
            var totalSaleVolume = CalculateTotalSalesVolume(activeFulfilledTradeOrders);

            var sizeOfVirtualPosition = totalPurchaseVolume - totalSaleVolume;
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
                    ?.Securities
                    ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, security.Identifiers));

            if (securityTick == null)
            {
                return CalculateInferredVirtualProfit(activeFulfilledTradeOrders, realisedRevenue, sizeOfVirtualPosition);
            }

            var virtualRevenue = securityTick?.Spread.Price.Value * sizeOfVirtualPosition;

            return realisedRevenue.GetValueOrDefault(0) + virtualRevenue.GetValueOrDefault(0);
        }

        private decimal? CalculateRealisedRevenue
            (IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.SellLong)
                    .Select(afto => afto.Volume * afto.ExecutedPrice)
                    .Sum();
        }

        private int CalculateTotalPurchaseVolume(
            IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.Position == OrderPosition.BuyLong)
                .Select(afto => afto.Volume)
                .Sum();
        }

        private int CalculateTotalSalesVolume(
            IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.Position == OrderPosition.SellLong)
                .Select(afto => afto.Volume)
                .Sum();
        }

        private decimal CalculateInferredVirtualProfit(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            decimal? realisedRevenue,
            int sizeOfVirtualPosition)
        {
            _logger.LogInformation(
                $"High Profit Rule - did not have access to exchange data. Attempting to infer the best price to use when pricing the virtual component of the profits.");

            var mostRecentTrade =
                activeFulfilledTradeOrders
                    .Where(afto => afto.ExecutedPrice != null)
                    .OrderByDescending(afto => afto.StatusChangedOn)
                    .FirstOrDefault();

            if (mostRecentTrade == null)
            {
                return realisedRevenue.GetValueOrDefault(0);
            }

            var inferredVirtualProfits = mostRecentTrade.ExecutedPrice.GetValueOrDefault(0) * sizeOfVirtualPosition;

            return realisedRevenue.GetValueOrDefault(0) + inferredVirtualProfits;
        }

        protected override void Genesis()
        {
            _logger.LogDebug("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void MarketOpen(StockExchange exchange)
        {
            _logger.LogDebug($"Trading Opened for exchange {exchange.Name} in the High Profit Rule");
            _marketOpened = true;
        }

        protected override void MarketClose(StockExchange exchange)
        {
            _logger.LogDebug($"Trading closed for exchange {exchange.Name} in the High Profit Rule. Running market closure virtual profits check.");

            RunRuleForAllTradingHistories();
            _marketOpened = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug("Universe Eschaton occurred in the High Profit Rule");
            if (_marketOpened)
            {
                RunRuleForAllTradingHistories();
            }
        }
    }
}
