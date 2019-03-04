using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Calculators.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        protected readonly ILogger<HighProfitsRule> Logger;
        private readonly IHighProfitsRuleEquitiesParameters _equitiesParameters;
        protected readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        protected readonly IUniverseAlertStream _alertStream;

        private readonly ICostCalculatorFactory _costCalculatorFactory;
        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;
        private readonly IMarketDataCacheStrategyFactory _marketDataCacheFactory;
        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private bool _hasMissingData = false;
        protected bool MarketClosureRule = false;

        public HighProfitStreamRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.WindowSize ?? TimeSpan.FromHours(8),
                Domain.Scheduling.Rules.HighProfits,
                EquityRuleHighProfitFactory.Version,
                "High Profit Rule",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _marketDataCacheFactory = marketDataCacheFactory ?? throw new ArgumentNullException(nameof(marketDataCacheFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator 
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

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

            var targetCurrency = new Domain.Financial.Currency(_equitiesParameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency =
                liveTrades.Any()
                 && (liveTrades.All(x =>
                     string.Equals(
                         x.OrderCurrency.Value,
                         targetCurrency.Value,
                         StringComparison.InvariantCultureIgnoreCase)));

            var costCalculator = GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var marketCache =
                MarketClosureRule
                    ? _marketDataCacheFactory.InterdayStrategy(UniverseEquityInterdayCache)
                    : _marketDataCacheFactory.IntradayStrategy(UniverseEquityIntradayCache);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, UniverseDateTime, _ruleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(liveTrades, UniverseDateTime, _ruleCtx, marketCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                _hasMissingData = true;
                return;
            }

            var revenue = revenueResponse.Money;

            if (revenue == null)
            {
                Logger.LogInformation($"rule had null for revenues for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");

                return;
            }

            if (cost == null)
            {
                Logger.LogError($"something went wrong. We have calculable revenues but not costs for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");
                return;
            }

            if (revenue.Value.Value <= 0
                || cost.Value.Value <= 0)
            {
                Logger.LogInformation($"had revenue of {revenue.Value.Value} and cost of {cost.Value.Value}. Returning.");
                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = (revenue.Value.Value / cost.Value.Value) - 1;

            var hasHighProfitAbsolute = HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (_equitiesParameters.UseCurrencyConversions
                && !string.IsNullOrEmpty(_equitiesParameters.HighProfitCurrencyConversionTargetCurrency))
            {
                Logger.LogInformation($"is set to use currency conversions and has a target conversion currency to {_equitiesParameters.HighProfitCurrencyConversionTargetCurrency}. Calling set exchange rate profits.");

                exchangeRateProfits = SetExchangeRateProfits(liveTrades);
            }

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                Logger.LogInformation($"had a breach for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

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
            var currency = new Domain.Financial.Currency(_equitiesParameters.HighProfitCurrencyConversionTargetCurrency);
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

        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, Domain.Financial.Currency targetCurrency)
        {
            if (!_equitiesParameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                Logger.LogInformation($"GetCostCalculator using non currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

                return _costCalculatorFactory.CostCalculator();
            }

            Logger.LogInformation($"GetCostCalculator using currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

            return _costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, Domain.Financial.Currency targetCurrency)
        {
            if (!_equitiesParameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Value))
            {
                var calculator =
                    MarketClosureRule
                        ? _revenueCalculatorFactory.RevenueCalculatorMarketClosureCalculator()
                        : _revenueCalculatorFactory.RevenueCalculator();

                Logger.LogInformation($"GetRevenueCalculator using non currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

                return calculator;
            }
           
            var currencyConvertingCalculator =
                MarketClosureRule
                    ? _revenueCalculatorFactory.RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                    : _revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(targetCurrency);

            Logger.LogInformation($"GetRevenueCalculator using currency conversion one for {targetCurrency.Value} at {UniverseDateTime}");

            return currencyConvertingCalculator;
        }

        protected override void RunInitialSubmissionRule(ITradingHistoryStack history)
        {
            // do nothing
        }

        private void WriteAlertToMessageSender(
            Stack<Order> activeTrades,
            Money absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage,
            IExchangeRateProfitBreakdown breakdown)
        {
            var security = activeTrades.FirstOrDefault(at => at?.Instrument != null)?.Instrument;

            Logger.LogInformation($"breach detected for {security?.Identifiers}. Writing breach to alert stream.");

            var position = new TradePosition(activeTrades.ToList());
            var breach =
                new HighProfitRuleBreach(
                    OrganisationFactorValue,
                    _ruleCtx.SystemProcessOperationContext(),
                    _ruleCtx.CorrelationId(),
                    _equitiesParameters,
                    absoluteProfit.Value,
                    absoluteProfit.Currency.Code,
                    profitRatio,
                    security,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    position,
                    MarketClosureRule,
                    breakdown);

            var alertEvent = new UniverseAlertEvent(Domain.Scheduling.Rules.HighProfits, breach, _ruleCtx);
            _alertStream.Add(alertEvent);
        }

        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return _equitiesParameters.HighProfitPercentageThreshold.HasValue
               && _equitiesParameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        private bool HasHighProfitAbsolute(Money absoluteProfits)
        {
            if (_equitiesParameters.HighProfitAbsoluteThreshold == null)
            {
                return false;
            }

            if (_equitiesParameters.UseCurrencyConversions
                && !string.Equals(
                _equitiesParameters.HighProfitCurrencyConversionTargetCurrency,
                absoluteProfits.Currency.Code,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _ruleCtx.EventException("had mismatching absolute profits currencies. Something went horribly wrong!");
                throw new InvalidOperationException("had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= _equitiesParameters.HighProfitAbsoluteThreshold;
        }    
        
        protected override void Genesis()
        {
            Logger.LogInformation("Universe Genesis occurred");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            Logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            Logger.LogInformation($"Trading closed for exchange {exchange.MarketId}. Running market closure virtual profits check.");
        }

        protected override void EndOfUniverse()
        {
            Logger.LogInformation("Universe Eschaton occurred.");

            if (!MarketClosureRule)
            {
                RunRuleForAllTradingHistories();
            }
            
            if (_hasMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                Logger.LogInformation($"Deleting alerts off the message sender");
                var alert = new UniverseAlertEvent(Domain.Scheduling.Rules.HighProfits, null, _ruleCtx, false, true);
                _alertStream.Add(alert);

                _dataRequestSubscriber.SubmitRequest();

                _ruleCtx?.EndEvent();
            }
            else
            {
                _ruleCtx?.EndEvent();
            }
        }

        public virtual IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighProfitStreamRule)Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (HighProfitStreamRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }
    }
}
