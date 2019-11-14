using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Currency.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    public class RevenueCurrencyConvertingCalculator : IRevenueCalculator
    {
        protected readonly IMarketTradingHoursService TradingHoursService;
        private readonly Domain.Core.Financial.Money.Currency _targetCurrency;
        private readonly ICurrencyConverterService _currencyConverterService;
        protected readonly ILogger Logger;

        public RevenueCurrencyConvertingCalculator(
            Domain.Core.Financial.Money.Currency targetCurrency,
            ICurrencyConverterService currencyConverterService,
            IMarketTradingHoursService tradingHoursService,
            ILogger<RevenueCurrencyConvertingCalculator> logger)
        {
            _targetCurrency = targetCurrency;
            _currencyConverterService = currencyConverterService ?? throw new ArgumentNullException(nameof(currencyConverterService));
            TradingHoursService = tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
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
                return new RevenueMoney(false, realisedRevenue, HighProfitComponents.Realised);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Could not find a security so just returning the realised revenue.");

                return new RevenueMoney(false, realisedRevenue, HighProfitComponents.Realised);
            }

            var marketDataRequest =
                MarketDataRequest(
                    activeFulfilledTradeOrders.FirstOrDefault()?.Market?.MarketIdentifierCode,
                    security.Identifiers,
                    universeDateTime,
                    ctx,
                    cacheStrategy.DataSource);

            if (marketDataRequest == null)
            {
                Logger.LogInformation($"RevenueCurrencyConvertingCalculator CalculateRevenueOfPosition had no virtual position. Total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Returning the realised revenue only.");

                return new RevenueMoney(false, null, HighProfitComponents.Realised);
            }

            var marketResponse = cacheStrategy.Query(marketDataRequest);

            if (marketResponse.HadMissingData())
            {
                Logger.LogInformation($"Revenue currency converting calculator calculating for inferred virtual profits due to missing market data");

                return new RevenueMoney(true, null, HighProfitComponents.Virtual);
            }

            var virtualRevenue = (marketResponse.PriceOrClose()?.Value ?? 0) * sizeOfVirtualPosition;
            var money = new Money(virtualRevenue, marketResponse.PriceOrClose()?.Currency.Code ?? string.Empty);
            var convertedVirtualRevenues = await _currencyConverterService.Convert(new[] { money }, _targetCurrency, universeDateTime, ctx);

            if (realisedRevenue == null
                && convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate converted virtual revenues, returning null.");
                return null;
            }

            if (realisedRevenue == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate realised revenues, returning unrealised revenues.");
                return new RevenueMoney(false, convertedVirtualRevenues, HighProfitComponents.Virtual);
            }

            if (convertedVirtualRevenues == null)
            {
                Logger.LogInformation($"Revenue currency converting calculator could not calculate virtual revenues. Returning realised revenues.");
                return new RevenueMoney(false, realisedRevenue, HighProfitComponents.Realised);
            }

            if (!realisedRevenue.Value.DenominatedInCommonCurrency(convertedVirtualRevenues.Value))
            {
                var convertedMoney = 
                    await this._currencyConverterService.Convert(
                        new[] { convertedVirtualRevenues.Value },
                        realisedRevenue.Value.Currency,
                        universeDateTime,
                        ctx);

                if (convertedMoney == null)
                {
                    this.Logger.LogError($"CalculateRevenueOfPosition at {universeDateTime} was unable to convert ({convertedVirtualRevenues.Value.Currency}) {convertedVirtualRevenues.Value.Value} to the realised revenue currency of {realisedRevenue.Value.Currency} due to missing currency conversion data.");

                    return new RevenueMoney(true, realisedRevenue, HighProfitComponents.Realised);
                }

                convertedVirtualRevenues = convertedMoney.Value;
            }
            
            var totalMoneys = realisedRevenue + convertedVirtualRevenues;

            var component =
                realisedRevenue.HasValue && realisedRevenue.Value.Value > 0
                ? HighProfitComponents.Hybrid
                : HighProfitComponents.Virtual;

            return new RevenueMoney(false, totalMoneys, component);
        }

        private async Task<Money?> CalculateRealisedRevenue
            (IList<Order> activeFulfilledTradeOrders,
            Domain.Core.Financial.Money.Currency targetCurrency,
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

            var conversion = await _currencyConverterService.Convert(filledOrders, targetCurrency, universeDateTime, ruleCtx);

            return conversion;
        }

        private decimal CalculateTotalPurchaseVolume(
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

        private decimal CalculateTotalSalesVolume(
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
            ISystemProcessOperationRunRuleContext ctx,
            DataSource dataSource)
        {
            var tradingHours = TradingHoursService.GetTradingHoursForMic(mic);
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
                ctx?.Id(),
                dataSource);
        }
    }
}
