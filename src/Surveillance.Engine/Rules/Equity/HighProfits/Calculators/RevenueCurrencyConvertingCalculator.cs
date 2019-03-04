using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core.Financial;
using Domain.Markets;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    public class RevenueCurrencyConvertingCalculator : IRevenueCalculator
    {
        protected readonly IMarketTradingHoursManager TradingHoursManager;
        private readonly Domain.Core.Financial.Currency _targetCurrency;
        private readonly ICurrencyConverter _currencyConverter;
        protected readonly ILogger Logger;

        public RevenueCurrencyConvertingCalculator(
            Domain.Core.Financial.Currency targetCurrency,
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
        public async Task<RevenueMoney> CalculateRevenueOfPosition(
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
                return new RevenueMoney(false, realisedRevenue);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Could not find a security so just returning the realised revenue.");
                return new RevenueMoney(false, realisedRevenue);

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
                return new RevenueMoney(false, null);

            }

            var marketResponse = cacheStrategy.Query(marketDataRequest);

            if (marketResponse.HadMissingData())
            {
                Logger.LogInformation($"Revenue currency converting calculator calculating for inferred virtual profits due to missing market data");

                return new RevenueMoney(true, null);
            }

            var virtualRevenue = (marketResponse.PriceOrClose()?.Value ?? 0) * sizeOfVirtualPosition;
            var money = new Money(virtualRevenue, marketResponse.PriceOrClose()?.Currency.Code ?? string.Empty);
            var convertedVirtualRevenues = await _currencyConverter.Convert(new[] { money }, _targetCurrency, universeDateTime, ctx);

            if (realisedRevenue == null
                && convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate converted virtual revenues, returning null.");
                return null;
            }

            if (realisedRevenue == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate realised revenues, returning realised revenues.");
                return new RevenueMoney(false, convertedVirtualRevenues);
            }

            if (convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate virtual revenues. Returning virtual revenues.");
                return new RevenueMoney(false, realisedRevenue);
            }

            var totalMoneys = realisedRevenue + convertedVirtualRevenues;

            return new RevenueMoney(false, totalMoneys);
        }

        private async Task<Money?> CalculateRealisedRevenue
            (IList<Order> activeFulfilledTradeOrders,
            Domain.Core.Financial.Currency targetCurrency,
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
                    new Money(
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
