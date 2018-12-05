using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Trades.Orders;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingCalculator : IRevenueCalculator
    {
        private readonly DomainV2.Financial.Currency _targetCurrency;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ILogger _logger;

        public RevenueCurrencyConvertingCalculator(
            DomainV2.Financial.Currency targetCurrency,
            ICurrencyConverter currencyConverter,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
        {
            _targetCurrency = targetCurrency;
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
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

            var realisedRevenue = await CalculateRealisedRevenue(activeFulfilledTradeOrders, _targetCurrency, universeDateTime, ctx);
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
                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                    _targetCurrency,
                    universeDateTime,
                    ctx);
            }

            if (!latestExchangeFrameBook.ContainsKey(marketId))
            {
                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                    _targetCurrency,
                    universeDateTime,
                    ctx);
            }

            latestExchangeFrameBook.TryGetValue(marketId, out var frame);

            var securityTick =
                frame
                    ?.Securities
                    ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, security.Identifiers));

            if (securityTick == null)
            {
                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                   _targetCurrency,
                    universeDateTime,
                    ctx);
            }

            var virtualRevenue = (SecurityTickToPrice(securityTick)?.Value ?? 0) * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, securityTick.Spread.Price.Currency);
            var convertedVirtualRevenues = await _currencyConverter.Convert(new[] { currencyAmount }, _targetCurrency, universeDateTime, ctx);

            if (realisedRevenue == null
                && convertedVirtualRevenues == null)
            {
                return null;
            }

            if (realisedRevenue == null)
            {
                return convertedVirtualRevenues;
            }

            if (convertedVirtualRevenues == null)
            {
                return realisedRevenue;
            }

            return realisedRevenue + convertedVirtualRevenues;
        }

        private async Task<CurrencyAmount?> CalculateRealisedRevenue
            (IList<TradeOrderFrame> activeFulfilledTradeOrders,
            DomainV2.Financial.Currency targetCurrency,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ruleCtx)
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

            var conversion = await _currencyConverter.Convert(filledOrders, targetCurrency, universeDateTime, ruleCtx);

            return conversion;
        }

        private int CalculateTotalPurchaseVolume(
            IList<TradeOrderFrame> activeFulfilledTradeOrders)
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

        private int CalculateTotalSalesVolume(
            IList<TradeOrderFrame> activeFulfilledTradeOrders)
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

        private async Task<CurrencyAmount?> CalculateInferredVirtualProfit(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            CurrencyAmount? realisedRevenue,
            int sizeOfVirtualPosition,
            DomainV2.Financial.Currency targetCurrency,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
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
            var convertedCurrencyAmount = await _currencyConverter.Convert(new[] { currencyAmount }, targetCurrency, universeDateTime, ctx);

            if (realisedRevenue == null
                && convertedCurrencyAmount == null)
            {
                return null;
            }

            if (realisedRevenue == null)
            {
                return convertedCurrencyAmount;
            }

            if (convertedCurrencyAmount == null)
            {
                return realisedRevenue;
            }

            return realisedRevenue + convertedCurrencyAmount;
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
