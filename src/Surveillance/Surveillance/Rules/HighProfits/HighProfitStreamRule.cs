using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Currency.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        private readonly ILogger<HighProfitsRule> _logger;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IHighProfitRuleCachedMessageSender _sender;
        private readonly IHighProfitsRuleParameters _parameters;
        private readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        private readonly bool _submitRuleBreaches;

        protected bool MarketClosureRule = false;

        public HighProfitStreamRule(
            ICurrencyConverter currencyConverter,
            IHighProfitRuleCachedMessageSender sender,
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            bool submitRuleBreaches,
            ILogger<HighProfitsRule> logger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromHours(8),
                Domain.Scheduling.Rules.HighProfits,
                Versioner.Version(1, 0),
                "High Profit Rule",
                ruleCtx,
                logger)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _submitRuleBreaches = submitRuleBreaches;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual bool RunRuleGuard(ITradingHistoryStack history)
        {
            return true;
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            RunRuleGuard(history);

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at =>
                    at.OrderStatus == OrderStatus.PartialFulfilled
                    || at.OrderStatus == OrderStatus.Fulfilled)
                .ToList();

            var targetCurrency = new Domain.Finance.Currency(_parameters.HighProfitCurrencyConversionTargetCurrency);

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
                _logger.LogInformation($"High profit rules had revenue of {revenue.Value.Value} and cost of {cost.Value.Value}. Returning.");
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

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private void WriteAlertToMessageSender(
            Stack<TradeOrderFrame> activeTrades,
            decimal absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage)
        {
            var security = activeTrades.FirstOrDefault(at => at.Security != null)?.Security;

            _logger.LogInformation($"High Profits Rule breach detected for {security?.Identifiers}. Writing breach to message sender.");

            var position = new TradePosition(activeTrades.ToList());
            var breach =
                new HighProfitRuleBreach(
                    _parameters,
                    absoluteProfit,
                    _parameters.HighProfitCurrencyConversionTargetCurrency,
                    profitRatio,
                    security,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    position,
                    MarketClosureRule);

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
                _parameters.HighProfitCurrencyConversionTargetCurrency,
                absoluteProfits.Currency.Value,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _ruleCtx.EventException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
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

            var adjustedToCurrencyPurchaseOrders =
                await _currencyConverter.Convert(purchaseOrders, targetCurrency, UniverseDateTime, _ruleCtx);

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

            var marketId = activeFulfilledTradeOrders.FirstOrDefault()?.Market?.Id;
            if (marketId == null)
            {
                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                    targetCurrency);
            }

            if (!LatestExchangeFrameBook.ContainsKey(marketId))
            {
                return await CalculateInferredVirtualProfit(
                    activeFulfilledTradeOrders,
                    realisedRevenue,
                    sizeOfVirtualPosition,
                    targetCurrency);
            }

            LatestExchangeFrameBook.TryGetValue(marketId, out var frame);

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
                    targetCurrency);
            }

            var virtualRevenue = (SecurityTickToPrice(securityTick)?.Value ?? 0) * sizeOfVirtualPosition;
            var currencyAmount = new CurrencyAmount(virtualRevenue, securityTick.Spread.Price.Currency);
            var convertedVirtualRevenues = await _currencyConverter.Convert(new[] { currencyAmount }, targetCurrency, UniverseDateTime, _ruleCtx);

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

            var conversion = await _currencyConverter.Convert(filledOrders, targetCurrency, UniverseDateTime, _ruleCtx);

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
            var convertedCurrencyAmount = await _currencyConverter.Convert(new[] { currencyAmount }, targetCurrency, UniverseDateTime, _ruleCtx);

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

        protected virtual Price? SecurityTickToPrice(SecurityTick tick)
        {
            if (tick == null)
            {
                return null;
            }

            return tick.Spread.Price;
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading Opened for exchange {exchange.MarketId} in the High Profit Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Trading closed for exchange {exchange.MarketId} in the High Profit Rule. Running market closure virtual profits check.");
        }

        protected override void EndOfUniverse()
        {
            _logger.LogInformation("Universe Eschaton occurred in the High Profit Rule");

            if (!MarketClosureRule)
            {
                RunRuleForAllTradingHistories();
            }

            int alerts = 0;
            if (_submitRuleBreaches)
            {
                alerts = _sender.Flush(_ruleCtx);
            }

            _ruleCtx.UpdateAlertEvent(alerts);
            _ruleCtx?.EndEvent();
        }
    }
}
