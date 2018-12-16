using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;
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
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IUniverseMarketCache universeMarketCache)
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
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                return realisedRevenue;
            }

            var marketDataRequest =
                new MarketDataRequest(
                    activeFulfilledTradeOrders.FirstOrDefault()?.Market?.MarketIdentifierCode,
                    security.Identifiers,
                    null,
                    null);

            var marketResponse = universeMarketCache.Get(marketDataRequest);

            if (marketResponse.HadMissingData)
            {
                _logger.LogInformation($"Revenue currency converting calculator calculating for inferred virtual profits due to missing market data");

                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                    _targetCurrency,
                    universeDateTime,
                    ctx);
            }

            var securityTick = marketResponse.Response;
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
            (IList<Order> activeFulfilledTradeOrders,
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
                .Where(afto => afto.OrderPosition == OrderPositions.SELL
                               || afto.OrderPosition == OrderPositions.COVER)
                .Select(afto =>
                    new CurrencyAmount(
                        afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAveragePrice.GetValueOrDefault().Value,
                        afto.OrderCurrency))
                .ToList();

            var conversion = await _currencyConverter.Convert(filledOrders, targetCurrency, universeDateTime, ruleCtx);

            return conversion;
        }

        private long CalculateTotalPurchaseVolume(
            IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.OrderPosition == OrderPositions.BUY
                               || afto.OrderPosition == OrderPositions.SHORT)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        private long CalculateTotalSalesVolume(
            IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(afto => afto.OrderPosition == OrderPositions.SELL
                               || afto.OrderPosition == OrderPositions.COVER)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        private async Task<CurrencyAmount?> CalculateInferredVirtualProfit(
            IList<Order> activeFulfilledTradeOrders,
            CurrencyAmount? realisedRevenue,
            long sizeOfVirtualPosition,
            DomainV2.Financial.Currency targetCurrency,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
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
