using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Finance;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Rules.High_Profits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : BaseUniverseRule, IHighProfitRule
    {
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IHighProfitRuleCachedMessageSender _sender;
        private readonly IHighProfitsRuleParameters _parameters;

        private bool _marketOpened = true; // assume the market has opened initially

        public HighProfitsRule(
            ICurrencyConverter currencyConverter,
            IHighProfitRuleCachedMessageSender sender,
            IHighProfitsRuleParameters parameters,
            ILogger<HighProfitsRule> logger) 
            : base(
                parameters?.WindowSize ?? TimeSpan.FromHours(8),
                Domain.Scheduling.Rules.HighProfits,
                Versioner.Version(1, 0),
                "High Profit Rule",
                logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at =>
                    at.OrderStatus == OrderStatus.PartialFulfilled
                    || at.OrderStatus == OrderStatus.Fulfilled)
                .ToList();

            var targetCurrency = new Domain.Finance.Currency(_parameters.HighProfitAbsoluteThresholdCurrency);

            var costTask = CalculateCostOfPosition(liveTrades, targetCurrency);
            var revenueTask = CalculateRevenueOfPosition(liveTrades, targetCurrency);

            costTask.Wait();
            revenueTask.Wait();

            var cost = costTask.Result;
            var revenue = revenueTask.Result;

            if (revenue == null)
            {
                return;
            }

            if (cost == null)
            {
                _logger.LogError("High profits rule - something went horribly wrong. We have calculable revenues but not costs");
                return;
            }

            if (revenue.Value.Value <= 0
                || cost.Value.Value <= 0)
            {
                _logger.LogDebug($"High profit rules had revenue of {revenue.Value.Value} and cost of {cost.Value.Value}. Returning.");
                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = (revenue.Value.Value / cost.Value.Value) - 1;

            var hasHighProfitAbsolute = HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = HasHighProfitPercentage(profitRatio);

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                WriteAlertToMessageSender(activeTrades, absoluteProfit.Value, profitRatio, hasHighProfitAbsolute, hasHighProfitPercentage);
            }
        }

        private void WriteAlertToMessageSender(
            Stack<TradeOrderFrame> activeTrades,
            decimal absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage)
        {
            var security = activeTrades.FirstOrDefault(at => at.Security != null)?.Security;

            _logger.LogDebug($"High Profits Rule breach detected for {security?.Identifiers}. Writing breach to message sender.");

            var position = new TradePosition(activeTrades.ToList());
            var breach =
                new HighProfitRuleBreach(
                    _parameters,
                    absoluteProfit,
                    _parameters.HighProfitAbsoluteThresholdCurrency,
                    profitRatio,
                    security,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    position);

            _sender.Send(breach);
        }

        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return _parameters.HighProfitPercentageThreshold.HasValue
               && _parameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        private bool HasHighProfitAbsolute(CurrencyAmount absoluteProfits)
        {
            if (_parameters.HighProfitAbsoluteThreshold == null)
            {
                return false;
            }

            if (!string.Equals(
                _parameters.HighProfitAbsoluteThresholdCurrency,
                absoluteProfits.Currency.Value,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= _parameters.HighProfitAbsoluteThreshold;
        }

        /// <summary>
        /// Sum the total buy in for the position
        /// </summary>
        private async Task<CurrencyAmount?> CalculateCostOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            Domain.Finance.Currency targetCurrency)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return null;
            }

            var purchaseOrders =
                activeFulfilledTradeOrders
                    .Where(afto => afto.Position == OrderPosition.Buy)
                    .Select(afto => new CurrencyAmount(afto.FulfilledVolume * afto.ExecutedPrice?.Value ?? 0, afto.OrderCurrency))
                    .ToList();

            var adjustedToCurrencyPurchaseOrders = await _currencyConverter.Convert(purchaseOrders, targetCurrency, UniverseDateTime);

            return adjustedToCurrencyPurchaseOrders;
        }

        /// <summary>
        /// Take realised profits and then for any remaining amount use virtual profits
        /// </summary>
        private async Task<CurrencyAmount?> CalculateRevenueOfPosition(
            IList<TradeOrderFrame> activeFulfilledTradeOrders,
            Domain.Finance.Currency targetCurrency)
        {
            if (activeFulfilledTradeOrders == null
                || !activeFulfilledTradeOrders.Any())
            {
                return null;
            }

            var realisedRevenue = await CalculateRealisedRevenue(activeFulfilledTradeOrders, targetCurrency);
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

            var securityTick =
                LatestExchangeFrame
                    ?.Securities
                    ?.FirstOrDefault(sec => Equals(sec.Security.Identifiers, security.Identifiers));

            if (securityTick == null)
            {
                return await CalculateInferredVirtualProfit(activeFulfilledTradeOrders, realisedRevenue, sizeOfVirtualPosition, targetCurrency);
            }

            var virtualRevenue = securityTick.Spread.Price.Value * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, securityTick.Spread.Price.Currency);
            var convertedVirtualRevenues = await _currencyConverter.Convert(new[] { currencyAmount }, targetCurrency, UniverseDateTime);

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
            Domain.Finance.Currency targetCurrency)
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

            var conversion = await _currencyConverter.Convert(filledOrders, targetCurrency, UniverseDateTime);

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
            Domain.Finance.Currency targetCurrency)
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
            var convertedCurrencyAmount = await _currencyConverter.Convert(new[] { currencyAmount }, targetCurrency, UniverseDateTime);

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

        protected override void Genesis()
        {
            _logger.LogDebug("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Trading Opened for exchange {exchange.MarketId} in the High Profit Rule");
            _marketOpened = true;
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogDebug($"Trading closed for exchange {exchange.MarketId} in the High Profit Rule. Running market closure virtual profits check.");

            RunRuleForAllTradingHistories(exchange.MarketClose);
            _marketOpened = false;
        }

        protected override void EndOfUniverse()
        {
            _logger.LogDebug("Universe Eschaton occurred in the High Profit Rule");
            if (_marketOpened)
            {
                RunRuleForAllTradingHistories();
            }

            _sender.Flush();
        }
    }
}