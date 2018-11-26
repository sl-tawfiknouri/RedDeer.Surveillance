using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Market;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    /// <summary>
    /// Calculates revenues without attempting to convert currencies
    /// </summary>
    public class RevenueCalculator : IRevenueCalculator
    {
        private readonly ILogger _logger;

        public RevenueCalculator(ILogger<RevenueCalculator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Take realised profits and then for any remaining amount use virtual profits
        /// </summary>
        public async Task<CurrencyAmount?> CalculateRevenueOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IDictionary<Market.MarketId, ExchangeFrame> latestExchangeFrameBook)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return null;
            }

            var realisedRevenue = CalculateRealisedRevenue(activeFulfilledTradeOrders);
            var totalPurchaseVolume = CalculateTotalPurchaseVolume(activeFulfilledTradeOrders);
            var totalSaleVolume = CalculateTotalSalesVolume(activeFulfilledTradeOrders);

            var sizeOfVirtualPosition = totalPurchaseVolume - totalSaleVolume;
            if (sizeOfVirtualPosition <= 0)
            {
                // fully traded out position; return its value
                return realisedRevenue;
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Security;
            if (security == null)
            {
                return realisedRevenue;
            }

            var marketId = activeFulfilledTradeOrders.FirstOrDefault()?.Market?.Id;
            if (marketId == null)
            {
                return CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition);
            }

            if (!latestExchangeFrameBook.ContainsKey(marketId))
            {
                return CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition);
            }

            latestExchangeFrameBook.TryGetValue(marketId, out var frame);

            var securityTick =
                frame
                    ?.Securities
                    ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, security.Identifiers));

            if (securityTick == null)
            {
                return CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition);
            }

            var virtualRevenue = (SecurityTickToPrice(securityTick)?.Value ?? 0) * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, securityTick.Spread.Price.Currency);

            if (realisedRevenue == null)
            {
                return currencyAmount;
            }

            return realisedRevenue + currencyAmount;
        }

        private CurrencyAmount? CalculateRealisedRevenue(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return null;
            }

            var filledOrders =
                activeFulfilledTradeOrders
                .Where(afto => afto.Position == OrderPosition.Sell)
                .Select(afto =>
                    new CurrencyAmount(
                        afto.FulfilledVolume * afto.ExecutedPrice?.Value ?? 0,
                        afto.OrderCurrency))
                .ToList();

            if (!filledOrders.Any())
            {
                return null;
            }

            var summedCurrency = filledOrders.Aggregate((x, y) => new CurrencyAmount(x.Value + y.Value, x.Currency));

            return summedCurrency;
        }

        private int CalculateTotalPurchaseVolume(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.Position == OrderPosition.Buy)
                .Select(afto => afto.FulfilledVolume)
                .Sum();
        }

        private int CalculateTotalSalesVolume(IList<TradeOrderFrame> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.Position == OrderPosition.Sell)
                .Select(afto => afto.FulfilledVolume)
                .Sum();
        }

        private CurrencyAmount? CalculateInferredVirtualProfit(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            CurrencyAmount? realisedRevenue,
            int sizeOfVirtualPosition)
        {
            _logger.LogInformation(
                "High Profit Rule - did not have access to exchange data. Attempting to infer the best price to use when pricing the virtual component of the profits.");

            var mostRecentTrade =
                activeFulfilledTradeOrders
                    .Where(afto => afto.ExecutedPrice != null)
                    .OrderByDescending(afto => afto.StatusChangedOn)
                    .FirstOrDefault();

            if (mostRecentTrade == null)
            {
                return realisedRevenue;
            }

            var inferredVirtualProfits = mostRecentTrade.ExecutedPrice?.Value * sizeOfVirtualPosition ?? 0;
            var currencyAmount = new CurrencyAmount(inferredVirtualProfits, mostRecentTrade.OrderCurrency);

            if (realisedRevenue == null)
            {
                return currencyAmount;
            }

            return realisedRevenue + currencyAmount;
        }

        protected virtual Price? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.Spread.Price;
        }
    }
}
