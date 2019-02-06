using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Markets;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingCalculator : IRevenueCalculator
    {
        protected readonly IMarketTradingHoursManager TradingHoursManager;
        private readonly DomainV2.Financial.Currency _targetCurrency;
        private readonly ICurrencyConverter _currencyConverter;
        protected readonly ILogger Logger;

        public RevenueCurrencyConvertingCalculator(
            DomainV2.Financial.Currency targetCurrency,
            ICurrencyConverter currencyConverter,
            IMarketTradingHoursManager tradingHoursManager,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
        {
            _targetCurrency = targetCurrency;
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            TradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Take realised profits and then for any remaining amount use virtual profits
        /// </summary>
        public async Task<RevenueCurrencyAmount> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx,
            IMarketDataCacheStrategy cacheStrategy)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had a null or empty active fulfilled trade orders. Returning.");
                return null;
            }

            var realisedRevenue = await CalculateRealisedRevenue(activeFulfilledTradeOrders, _targetCurrency, universeDateTime, ctx);
            var totalPurchaseVolume = CalculateTotalPurchaseVolume(activeFulfilledTradeOrders);
            var totalSaleVolume = CalculateTotalSalesVolume(activeFulfilledTradeOrders);

            var sizeOfVirtualPosition = totalPurchaseVolume - totalSaleVolume;
            if (sizeOfVirtualPosition <= 0)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Returning the realised revenue only.");
                // fully traded out position; return its value
                return new RevenueCurrencyAmount(false, realisedRevenue);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Could not find a security so just returning the realised revenue.");
                return new RevenueCurrencyAmount(false, realisedRevenue);

            }

            var marketDataRequest =
                MarketDataRequest(
                    activeFulfilledTradeOrders.FirstOrDefault()?.Market?.MarketIdentifierCode,
                    security.Identifiers,
                    universeDateTime,
                    ctx);

            if (marketDataRequest == null)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Returning the realised revenue only.");
                return new RevenueCurrencyAmount(false, null);

            }

            var marketResponse = cacheStrategy.Query(marketDataRequest);

            if (marketResponse.HadMissingData())
            {
                Logger.LogInformation($"Revenue currency converting calculator calculating for inferred virtual profits due to missing market data");

                return new RevenueCurrencyAmount(true, null);
            }

            var virtualRevenue = (marketResponse.PriceOrClose()?.Value ?? 0) * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, marketResponse.PriceOrClose()?.Currency.Value ?? string.Empty);
            var convertedVirtualRevenues = await _currencyConverter.Convert(new[] { currencyAmount }, _targetCurrency, universeDateTime, ctx);

            if (realisedRevenue == null
                && convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate converted virtual revenues, returning null.");
                return null;
            }

            if (realisedRevenue == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate realised revenues, returning realised revenues.");
                return new RevenueCurrencyAmount(false, convertedVirtualRevenues);
            }

            if (convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate virtual revenues. Returning virtual revenues.");
                return new RevenueCurrencyAmount(false, realisedRevenue);
            }

            var totalCurrencyAmounts = realisedRevenue + convertedVirtualRevenues;

            return new RevenueCurrencyAmount(false, totalCurrencyAmounts);
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
                .Where(afto => afto.OrderDirection == OrderDirections.SELL
                               || afto.OrderDirection == OrderDirections.SHORT)
                .Select(afto =>
                    new CurrencyAmount(
                        afto.OrderFilledVolume.GetValueOrDefault(0) * afto.OrderAverageFillPrice.GetValueOrDefault().Value,
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
                .Where(afto => afto.OrderDirection == OrderDirections.BUY
                               || afto.OrderDirection == OrderDirections.COVER)
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
                .Where(afto => afto.OrderDirection == OrderDirections.SELL
                               || afto.OrderDirection == OrderDirections.SHORT)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }
        
        protected virtual MarketDataRequest MarketDataRequest(
            string mic,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            var tradingHours = TradingHoursManager.GetTradingHoursForMic(mic);
            if (!tradingHours.IsValid)
            {
                Logger.LogError($"RevenueCurrencyConvertingCalculator was not able to get meaningful trading hours for the mic {mic}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                mic,
                string.Empty,
                identifiers,
                tradingHours.OpeningInUtcForDay(universeDateTime),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(universeDateTime),
                ctx?.Id());
        }
    }
}
