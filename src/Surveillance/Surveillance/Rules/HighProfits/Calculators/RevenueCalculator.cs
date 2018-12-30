﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Markets;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Calculators
{
    /// <summary>
    /// Calculates revenues without attempting to convert currencies
    /// </summary>
    public class RevenueCalculator : IRevenueCalculator
    {
        protected readonly IMarketTradingHoursManager TradingHoursManager;
        protected readonly ILogger Logger;

        public RevenueCalculator(IMarketTradingHoursManager tradingHoursManager, ILogger<RevenueCalculator> logger)
        {
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
            IUniverseMarketCache universeMarketCache)
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
                return new RevenueCurrencyAmount(false, realisedRevenue);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                return new RevenueCurrencyAmount(false, realisedRevenue);
            }

            var marketDataRequest = 
                MarketDataRequest(
                    activeFulfilledTradeOrders.First().Market.MarketIdentifierCode,
                    security.Identifiers,
                    universeDateTime,
                    ctx);

            if (marketDataRequest == null)
            {
                return new RevenueCurrencyAmount(false, null);
            }

            var marketDataResult = universeMarketCache.Get(marketDataRequest);
            if (marketDataResult.HadMissingData)
            {
                return new RevenueCurrencyAmount(true, null);
            }

            var securityTick = marketDataResult.Response;           
            var virtualRevenue = (SecurityTickToPrice(securityTick)?.Value ?? 0) * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, securityTick.Spread.Price.Currency);

            if (realisedRevenue == null)
            {
                return new RevenueCurrencyAmount(false, currencyAmount);
            }

            var totalCurrencyAmounts = realisedRevenue + currencyAmount;

            return new RevenueCurrencyAmount(false, totalCurrencyAmounts);
        }

        private CurrencyAmount? CalculateRealisedRevenue(IList<Order> activeFulfilledTradeOrders)
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
                .Where(afto => afto.OrderPosition == OrderPositions.BUY
                                || afto.OrderPosition == OrderPositions.SHORT)
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
                .Where(afto => afto.OrderPosition == OrderPositions.SELL
                               || afto.OrderPosition == OrderPositions.COVER)
                .Select(afto => afto.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        protected virtual MarketDataRequest MarketDataRequest(
            string mic,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ctx)
        {
            var tradingHours = TradingHoursManager.Get(mic);
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
