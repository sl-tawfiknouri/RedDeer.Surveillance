using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Trading;
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
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IDictionary<string, ExchangeFrame> latestExchangeFrameBook)
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
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                return realisedRevenue;
            }

            var marketId = activeFulfilledTradeOrders.FirstOrDefault()?.Market?.MarketIdentifierCode;
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

        private CurrencyAmount? CalculateRealisedRevenue(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return null;
            }

            var filledOrders =
                activeFulfilledTradeOrders
                .Where(afto => afto.OrderPosition == OrderPositions.SELL)
                .Select(afto =>
                    new CurrencyAmount(
                        afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAveragePrice.GetValueOrDefault().Value,
                        afto.OrderCurrency))
                .ToList();

            if (!filledOrders.Any())
            {
                return null;
            }

            var summedCurrency = filledOrders.Aggregate((x, y) => new CurrencyAmount(x.Value + y.Value, x.Currency));

            return summedCurrency;
        }

        private long CalculateTotalPurchaseVolume(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.OrderPosition == OrderPositions.BUY)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        private long CalculateTotalSalesVolume(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.OrderPosition == OrderPositions.SELL)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        private CurrencyAmount? CalculateInferredVirtualProfit(
            IList<Order> activeFulfilledTradeOrders,
            CurrencyAmount? realisedRevenue,
            long sizeOfVirtualPosition)
        {
            _logger.LogInformation(
                "High Profit Rule - did not have access to exchange data. Attempting to infer the best price to use when pricing the virtual component of the profits.");

            var mostRecentTrade =
                activeFulfilledTradeOrders
                    .Where(afto => afto.OrderAveragePrice != null)
                    .OrderByDescending(afto => afto.MostRecentDateEvent())
                    .FirstOrDefault();

            if (mostRecentTrade == null)
            {
                return realisedRevenue;
            }

            var inferredVirtualProfits = mostRecentTrade.OrderAveragePrice.GetValueOrDefault().Value * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(inferredVirtualProfits, mostRecentTrade.OrderCurrency);

            if (realisedRevenue == null)
            {
                return currencyAmount;
            }

            return realisedRevenue + currencyAmount;
        }

        protected virtual CurrencyAmount? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.Spread.Price;
        }
    }
}
