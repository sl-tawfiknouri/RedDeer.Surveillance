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
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators
{
    /// <summary>
    /// Calculates revenues without attempting to convert currencies
    /// </summary>
    public class RevenueCalculator : IRevenueCalculator
    {
        protected readonly IMarketTradingHoursService TradingHoursService;
        protected readonly ILogger Logger;

        public RevenueCalculator(IMarketTradingHoursService tradingHoursService, ILogger<RevenueCalculator> logger)
        {
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
            IMarketDataCacheStrategy marketCacheStrategy)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                Logger.LogInformation($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had null or empty active fulfilled trade orders.");
                return null;
            }

            var realisedRevenue = CalculateRealisedRevenue(activeFulfilledTradeOrders);
            var totalPurchaseVolume = CalculateTotalPurchaseVolume(activeFulfilledTradeOrders);
            var totalSaleVolume = CalculateTotalSalesVolume(activeFulfilledTradeOrders);

            var sizeOfVirtualPosition = totalPurchaseVolume - totalSaleVolume;
            if (sizeOfVirtualPosition <= 0)
            {
                Logger.LogInformation($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Returning realised profits only.");

                // fully traded out position; return its value
                return new RevenueMoney(false, realisedRevenue);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                Logger.LogWarning($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Was going to calculate a virtual position but could not find security information from the active fulfilled trade orders.");

                return new RevenueMoney(false, realisedRevenue);
            }

            var marketDataRequest = 
                MarketDataRequest(
                    activeFulfilledTradeOrders.First().Market.MarketIdentifierCode,
                    security.Identifiers,
                    universeDateTime,
                    ctx,
                    marketCacheStrategy.DataSource);

            if (marketDataRequest == null)
            {
                Logger.LogWarning($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had a null market data request. Returning null.");

                return new RevenueMoney(false, null);
            }

            var marketDataResult = marketCacheStrategy.Query(marketDataRequest);
            if (marketDataResult.HadMissingData())
            {
                Logger.LogWarning($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had missing market data so will be calculating the inferred virtual profits instead.");
                return new RevenueMoney(true, null);
            }

            var virtualRevenue = (marketDataResult.PriceOrClose()?.Value ?? 0) * sizeOfVirtualPosition;
            var money = new Money(virtualRevenue, marketDataResult.PriceOrClose()?.Currency.Code ?? string.Empty);

            if (realisedRevenue == null)
            {
                Logger.LogWarning($"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had a null value for realised revenue so returning virtual revenue only of ({money.Currency}) {money.Value}.");
                return new RevenueMoney(false, money);

            }

            var totalMoneys = realisedRevenue + money;

            return new RevenueMoney(false, totalMoneys);
        }

        private Money? CalculateRealisedRevenue(IList<Order> activeFulfilledTradeOrders)
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

            if (!filledOrders.Any())
            {
                return null;
            }

            var summedCurrency = filledOrders.Aggregate((x, y) => new Money(x.Value + y.Value, x.Currency));

            return summedCurrency;
        }

        private decimal CalculateTotalPurchaseVolume(IList<Order> activeFulfilledTradeOrders)
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

        private decimal CalculateTotalSalesVolume(IList<Order> activeFulfilledTradeOrders)
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
