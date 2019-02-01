using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Rules.HighProfits.Calculators.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;

namespace Surveillance.Rules.HighProfits
{
    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        protected readonly ILogger<HighProfitsRule> Logger;
        private readonly IHighProfitsRuleParameters _parameters;
        protected readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        protected readonly IUniverseAlertStream _alertStream;

        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private bool _hasMissingData = false;
        protected bool MarketClosureRule = false;

        public HighProfitStreamRule(
            IHighProfitsRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                parameters?.WindowSize ?? TimeSpan.FromHours(8),
                DomainV2.Scheduling.Rules.HighProfits,
                HighProfitRuleFactory.Version,
                "High Profit Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator 
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected virtual bool RunRuleGuard(ITradingHistoryStack history)
        {
            return true;
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunRule(ITradingHistoryStack history)
        {
            if (!RunRuleGuard(history))
            {
                return;
            }

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at => at.OrderStatus() == OrderStatus.Filled)
                .ToList();

            if (!liveTrades.Any())
            {
                return;
            }

            var targetCurrency = new DomainV2.Financial.Currency(_parameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency =
                liveTrades.Any()
                 && (liveTrades.All(x =>
                     string.Equals(
                         x.OrderCurrency.Value,
                         targetCurrency.Value,
                         StringComparison.InvariantCultureIgnoreCase)));

            var costCalculator = GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, UniverseDateTime, _ruleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(liveTrades, UniverseDateTime, _ruleCtx, UniverseEquityIntradayCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                _hasMissingData = true;
                return;
            }

            var revenue = revenueResponse.CurrencyAmount;

            if (revenue == null)
            {
                Logger.LogInformation($"High profits rule had null for revenues for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");

                return;
            }

            if (cost == null)
            {
                Logger.LogError($"High profits rule - something went wrong. We have calculable revenues but not costs for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");
                return;
            }

            if (revenue.Value.Value <= 0
                || cost.Value.Value <= 0)
            {
                Logger.LogInformation($"High profit rules had revenue of {revenue.Value.Value} and cost of {cost.Value.Value}. Returning.");
                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = (revenue.Value.Value / cost.Value.Value) - 1;

            var hasHighProfitAbsolute = HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (_parameters.UseCurrencyConversions
                && !string.IsNullOrEmpty(_parameters.HighProfitCurrencyConversionTargetCurrency))
            {
                Logger.LogInformation($"High profit rules is set to use currency conversions and has a target conversion currency to {_parameters.HighProfitCurrencyConversionTargetCurrency}. Calling set exchange rate profits.");

                exchangeRateProfits = SetExchangeRateProfits(liveTrades);
            }

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                Logger.LogInformation($"High profits rule had a breach for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

                WriteAlertToMessageSender(
                    activeTrades,
                    absoluteProfit,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits);
            }
        }

        private IExchangeRateProfitBreakdown SetExchangeRateProfits(List<Order> liveTrades)
        {
            var currency = new DomainV2.Financial.Currency(_parameters.HighProfitCurrencyConversionTargetCurrency);
            var buys = new TradePosition(liveTrades.Where(lt =>
                lt.OrderDirection == OrderDirections.BUY 
                || lt.OrderDirection == OrderDirections.COVER).ToList());

            var sells = new TradePosition(liveTrades.Where(lt =>
                lt.OrderDirection == OrderDirections.SELL
                || lt.OrderDirection == OrderDirections.SHORT).ToList());

            var exchangeRateProfitsTask =
                _exchangeRateProfitCalculator.ExchangeRateMovement(
                    buys,
                    sells,
                    currency,
                    _ruleCtx);

            var exchangeRateProfits = exchangeRateProfitsTask.Result;

            return exchangeRateProfits;
        }

        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, DomainV2.Financial.Currency targetCurrency)
        {
            if (!_parameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                Logger.LogInformation($"HighProfitStreamRule GetCostCalculator using non currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

                return _costCalculatorFactory.CostCalculator();
            }

            Logger.LogInformation($"HighProfitStreamRule GetCostCalculator using currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

            return _costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, DomainV2.Financial.Currency targetCurrency)
        {
            if (!_parameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                var calculator =
                    MarketClosureRule
                        ? _revenueCalculatorFactory.RevenueCalculatorMarkingTheClose()
                        : _revenueCalculatorFactory.RevenueCalculator();

                Logger.LogInformation($"HighProfitStreamRule GetRevenueCalculator using non currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

                return calculator;
            }
           
            var currencyConvertingCalculator =
                MarketClosureRule
                    ? _revenueCalculatorFactory.RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                    : _revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(targetCurrency);

            Logger.LogInformation($"HighProfitStreamRule GetRevenueCalculator using currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

            return currencyConvertingCalculator;
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private void WriteAlertToMessageSender(
            Stack<Order> activeTrades,
            CurrencyAmount absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage,
            IExchangeRateProfitBreakdown breakdown)
        {
            var security = activeTrades.FirstOrDefault(at => at?.Instrument != null)?.Instrument;

            Logger.LogInformation($"High Profits Rule breach detected for {security?.Identifiers}. Writing breach to alert stream.");

            var position = new TradePosition(activeTrades.ToList());
            var breach =
                new HighProfitRuleBreach(
                    _ruleCtx.SystemProcessOperationContext(),
                    _ruleCtx.CorrelationId(),
                    _parameters,
                    absoluteProfit.Value,
                    absoluteProfit.Currency.Value,
                    profitRatio,
                    security,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    position,
                    MarketClosureRule,
                    breakdown);

            var alertEvent = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
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

            if (_parameters.UseCurrencyConversions
                && !string.Equals(
                _parameters.HighProfitCurrencyConversionTargetCurrency,
                absoluteProfits.Currency.Value,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _ruleCtx.EventException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
                throw new InvalidOperationException("High profits rule had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= _parameters.HighProfitAbsoluteThreshold;
        }    
        
        protected override void Genesis()
        {
            Logger.LogInformation("Universe Genesis occurred in the High Profit Rule");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            Logger.LogInformation($"Trading Opened for exchange {exchange.MarketId} in the High Profit Rule");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            Logger.LogInformation($"Trading closed for exchange {exchange.MarketId} in the High Profit Rule. Running market closure virtual profits check.");
        }

        protected override void EndOfUniverse()
        {
            Logger.LogInformation("Universe Eschaton occurred in the High Profit Rule");

            if (!MarketClosureRule)
            {
                RunRuleForAllTradingHistories();
            }
            
            if (_hasMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                Logger.LogInformation($"High Profit Stream Rule deleting alerts off the message sender");
                var alert = new UniverseAlertEvent(DomainV2.Scheduling.Rules.HighProfits, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();

                var opCtx = _ruleCtx?.EndEvent();
                opCtx?.EndEventWithMissingDataError();
            }
            else
            {
                var opCtx = _ruleCtx?.EndEvent();
                opCtx?.EndEvent();
            }
        }

        public virtual object Clone()
        {
            var clone = (HighProfitStreamRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
