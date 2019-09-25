namespace Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators
{
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
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

    /// <summary>
    ///     Calculates revenues without attempting to convert currencies
    /// </summary>
    public class RevenueCalculator : IRevenueCalculator
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        protected readonly IMarketTradingHoursService TradingHoursService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevenueCalculator"/> class.
        /// </summary>
        /// <param name="tradingHoursService">
        /// The trading hours service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RevenueCalculator(
            IMarketTradingHoursService tradingHoursService,
            ILogger<RevenueCalculator> logger)
        {
            this.TradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The calculate revenue of position.
        /// </summary>
        /// <param name="activeFulfilledTradeOrders">
        /// The active fulfilled trade orders.
        /// </param>
        /// <param name="universeDateTime">
        /// The universe date time.
        /// </param>
        /// <param name="ruleRunContext">
        /// The rule run context.
        /// </param>
        /// <param name="marketCacheStrategy">
        /// The market cache strategy.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<RevenueMoney> CalculateRevenueOfPosition(
            IList<Order> activeFulfilledTradeOrders,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext ruleRunContext,
            IMarketDataCacheStrategy marketCacheStrategy)
        {
            if (activeFulfilledTradeOrders == null || !activeFulfilledTradeOrders.Any())
            {
                this.Logger.LogInformation(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had null or empty active fulfilled trade orders.");
                return null;
            }

            var realisedRevenue = this.CalculateRealisedRevenue(activeFulfilledTradeOrders);
            var totalPurchaseVolume = this.CalculateTotalPurchaseVolume(activeFulfilledTradeOrders);
            var totalSaleVolume = this.CalculateTotalSalesVolume(activeFulfilledTradeOrders);

            var sizeOfVirtualPosition = totalPurchaseVolume - totalSaleVolume;
            if (sizeOfVirtualPosition <= 0)
            {
                this.Logger.LogInformation(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Returning realised profits only.");

                // fully traded out position; return its value
                return new RevenueMoney(false, realisedRevenue, HighProfitComponents.Realised);
            }

            // has a virtual position; calculate its value
            var security = activeFulfilledTradeOrders.FirstOrDefault()?.Instrument;
            if (security == null)
            {
                this.Logger.LogWarning(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Was going to calculate a virtual position but could not find security information from the active fulfilled trade orders.");

                return new RevenueMoney(false, realisedRevenue, HighProfitComponents.Realised);
            }

            var marketDataRequest = this.MarketDataRequest(
                activeFulfilledTradeOrders.First().Market.MarketIdentifierCode,
                security.Identifiers,
                universeDateTime,
                ruleRunContext,
                marketCacheStrategy.DataSource);

            if (marketDataRequest == null)
            {
                this.Logger.LogWarning(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had a null market data request. Returning null.");

                return new RevenueMoney(false, null, HighProfitComponents.Realised);
            }

            var marketDataResult = marketCacheStrategy.Query(marketDataRequest);
            if (marketDataResult.HadMissingData())
            {
                this.Logger.LogWarning(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had missing market data so will be calculating the inferred virtual profits instead.");
                return new RevenueMoney(true, null, HighProfitComponents.Realised);
            }

            var virtualRevenue = (marketDataResult.PriceOrClose()?.Value ?? 0) * sizeOfVirtualPosition;
            var money = new Money(virtualRevenue, marketDataResult.PriceOrClose()?.Currency.Code ?? string.Empty);

            if (realisedRevenue == null)
            {
                this.Logger.LogWarning(
                    $"RevenueCalculator CalculateRevenueOfPosition at {universeDateTime} had a fully traded out position with a total purchase volume of {totalPurchaseVolume} and total sale volume of {totalSaleVolume}. Had a null value for realised revenue so returning virtual revenue only of ({money.Currency}) {money.Value}.");
                return new RevenueMoney(false, money, HighProfitComponents.Virtual);
            }

            var totalMoneys = realisedRevenue + money;

            var component = 
                (realisedRevenue.HasValue && realisedRevenue.Value.Value > 0) 
                ? HighProfitComponents.Hybrid 
                : HighProfitComponents.Virtual;

            return await Task.FromResult(new RevenueMoney(false, totalMoneys, component));
        }

        /// <summary>
        /// The market data request.
        /// </summary>
        /// <param name="marketIdentifierCode">
        /// The market identifier code.
        /// </param>
        /// <param name="identifiers">
        /// The identifiers.
        /// </param>
        /// <param name="universeDateTime">
        /// The universe date time.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="dataSource">
        /// The data source.
        /// </param>
        /// <returns>
        /// The <see cref="MarketDataRequest"/>.
        /// </returns>
        protected virtual MarketDataRequest MarketDataRequest(
            string marketIdentifierCode,
            InstrumentIdentifiers identifiers,
            DateTime universeDateTime,
            ISystemProcessOperationRunRuleContext context,
            DataSource dataSource)
        {
            var tradingHours = this.TradingHoursService.GetTradingHoursForMic(marketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.Logger.LogError(
                    $"RevenueCurrencyConvertingCalculator was not able to get meaningful trading hours for the mic {marketIdentifierCode}. Unable to proceed with currency conversions.");
                return null;
            }

            return new MarketDataRequest(
                marketIdentifierCode,
                string.Empty,
                identifiers,
                tradingHours.OpeningInUtcForDay(universeDateTime),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(universeDateTime),
                context?.Id(),
                dataSource);
        }

        /// <summary>
        /// The calculate realized revenue.
        /// </summary>
        /// <param name="activeFulfilledTradeOrders">
        /// The active fulfilled trade orders.
        /// </param>
        /// <returns>
        /// The <see cref="Money"/>.
        /// </returns>
        private Money? CalculateRealisedRevenue(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return null;
            }

            var filledOrders = activeFulfilledTradeOrders
                .Where(_ => 
                    _.OrderDirection == OrderDirections.SELL 
                    || _.OrderDirection == OrderDirections.SHORT)
                .Select(_ => 
                    new Money(
                        _.OrderFilledVolume.GetValueOrDefault(0)
                        * _.OrderAverageFillPrice.GetValueOrDefault().Value,
                        _.OrderCurrency))
                .ToList();

            if (!filledOrders.Any())
            {
                return null;
            }

            var summedCurrency = filledOrders.Aggregate((_, __) => new Money(_.Value + __.Value, _.Currency));

            return summedCurrency;
        }

        /// <summary>
        /// The calculate total purchase volume.
        /// </summary>
        /// <param name="activeFulfilledTradeOrders">
        /// The active fulfilled trade orders.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateTotalPurchaseVolume(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(_ => 
                    _.OrderDirection == OrderDirections.BUY 
                    || _.OrderDirection == OrderDirections.COVER)
                .Select(_ => _.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }

        /// <summary>
        /// The calculate total sales volume.
        /// </summary>
        /// <param name="activeFulfilledTradeOrders">
        /// The active fulfilled trade orders.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private decimal CalculateTotalSalesVolume(IList<Order> activeFulfilledTradeOrders)
        {
            if (!activeFulfilledTradeOrders?.Any() ?? true)
            {
                return 0;
            }

            return activeFulfilledTradeOrders
                .Where(_ => 
                    _.OrderDirection == OrderDirections.SELL 
                    || _.OrderDirection == OrderDirections.SHORT)
                .Select(_ => _.OrderFilledVolume.GetValueOrDefault(0))
                .Sum();
        }
    }
}