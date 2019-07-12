using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Judgement.Equity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Judgements.Interfaces;
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
        protected readonly IHighProfitsRuleEquitiesParameters _equitiesParameters;
        protected readonly ISystemProcessOperationRunRuleContext _ruleCtx;
        protected readonly IJudgementService _judgementService;

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
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory marketCacheFactory,
            IMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromHours(8),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.HighProfits,
                EquityRuleHighProfitFactory.Version,
                "High Profit Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            _equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            _ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            _costCalculatorFactory = costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            _revenueCalculatorFactory = revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            _marketDataCacheFactory = marketDataCacheFactory ?? throw new ArgumentNullException(nameof(marketDataCacheFactory));
            _exchangeRateProfitCalculator =
                exchangeRateProfitCalculator 
                ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            _dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            _judgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
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

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            EvaluateHighProfits(history, UniverseEquityIntradayCache);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            EvaluateHighProfits(history, FutureUniverseEquityIntradayCache);
        }

        protected void EvaluateHighProfits(ITradingHistoryStack history, IUniverseEquityIntradayCache intradayCache)
        {
            if (!RunRuleGuard(history))
            {
                return;
            }

            var orderUnderAnalysis = UniverseEvent.UnderlyingEvent as Order;

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades
                .Where(at => at.OrderStatus() == OrderStatus.Filled)
                .ToList();

            if (orderUnderAnalysis == null)
            {
                orderUnderAnalysis = activeTrades.LastOrDefault();
            }

            if (!liveTrades.Any())
            {
                SetNoLiveTradesJudgement(orderUnderAnalysis);

                return;
            }

            var targetCurrency = new Domain.Core.Financial.Money.Currency(_equitiesParameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency =
                liveTrades.Any()
                 && (liveTrades.All(x =>
                     string.Equals(
                         x.OrderCurrency.Code,
                         targetCurrency.Code,
                         StringComparison.InvariantCultureIgnoreCase)));

            var costCalculator = GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var marketCache =
                MarketClosureRule
                    ? _marketDataCacheFactory.InterdayStrategy(UniverseEquityInterdayCache)
                    : _marketDataCacheFactory.IntradayStrategy(intradayCache);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, UniverseDateTime, _ruleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(liveTrades, UniverseDateTime, _ruleCtx, marketCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                SetMissingMarketDataJudgement(orderUnderAnalysis);
                _hasMissingData = true;

                return;
            }

            var revenue = revenueResponse.Money;

            if (revenue == null
                || revenue.Value.Value <= 0)
            {
                Logger.LogInformation($"rule had null for revenues for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");

                NoRevenueOrCostJudgement(orderUnderAnalysis);
                return;
            }

            if (cost == null
                || cost.Value.Value <= 0)
            {
                Logger.LogError($"something went wrong. We have calculable revenues but not costs for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. Returning.");

                NoRevenueOrCostJudgement(orderUnderAnalysis);
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

            RuleBreachContext ruleBreachContext = null;

            if (hasHighProfitAbsolute
                || hasHighProfitPercentage)
            {
                Logger.LogInformation($"had a breach for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

                ruleBreachContext =
                    new RuleBreachContext(
                        _equitiesParameters.Windows.BackwardWindowSize + _equitiesParameters.Windows.FutureWindowSize,
                        new TradePosition(liveTrades),
                        liveTrades.FirstOrDefault(_ => _?.Instrument != null)?.Instrument,
                        _ruleCtx.IsBackTest(),
                        _ruleCtx.RuleParameterId(),
                        _ruleCtx.SystemProcessOperationContext().Id.ToString(),
                        _ruleCtx.CorrelationId(),
                        OrganisationFactorValue,
                        _equitiesParameters,
                        UniverseDateTime);
            }

            SetJudgementForFullAnalysis(
                absoluteProfit,
                profitRatio,
                hasHighProfitAbsolute,
                hasHighProfitPercentage,
                exchangeRateProfits,
                ruleBreachContext,
                orderUnderAnalysis);
        }

        private void SetJudgementForFullAnalysis(
            Money absoluteProfit,
            decimal profitRatio,
            bool hasHighProfitAbsolute,
            bool hasHighProfitPercentage,
            IExchangeRateProfitBreakdown exchangeRateProfits,
            RuleBreachContext ruleBreachContext,
            Order orderUnderAnalysis)
        {
            var absoluteHighProfit =
                hasHighProfitAbsolute
                ? absoluteProfit.Value
                : (decimal?)null;

            var absoluteHighProfitCurrency =
                hasHighProfitAbsolute
                    ? absoluteProfit.Currency.Code
                    : null;

            var percentageHighProfit =
                hasHighProfitPercentage
                    ? profitRatio
                    : (decimal?)null;

            var jsonParameters = JsonConvert.SerializeObject(_equitiesParameters);
            var judgement =
                new HighProfitJudgement(
                    _ruleCtx.RuleParameterId(),
                    _ruleCtx.CorrelationId(),
                    orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                    orderUnderAnalysis?.OrderId,
                    absoluteHighProfit,
                    absoluteHighProfitCurrency,
                    percentageHighProfit,
                    jsonParameters,
                    false,
                    false);

            _judgementService.Judgement(
                new HighProfitJudgementContext(
                    judgement,
                    hasHighProfitAbsolute || hasHighProfitPercentage,
                    ruleBreachContext,
                    _equitiesParameters,
                    absoluteProfit,
                    absoluteProfit.Currency.Symbol,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits));
        }

        private void NoRevenueOrCostJudgement(Order orderUnderAnalysis)
        {
            var noRevenueJsonParameters = JsonConvert.SerializeObject(_equitiesParameters);
            var noRevenueJudgement =
                new HighProfitJudgement(
                    _ruleCtx.RuleParameterId(),
                    _ruleCtx.CorrelationId(),
                    orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                    orderUnderAnalysis?.OrderId,
                    null,
                    null,
                    null,
                    noRevenueJsonParameters,
                    false,
                    false);

            _judgementService.Judgement(new HighProfitJudgementContext(noRevenueJudgement, false));
        }

        private void SetMissingMarketDataJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(_equitiesParameters);
            var noTradesJudgement =
                new HighProfitJudgement(
                    _ruleCtx.RuleParameterId(),
                    _ruleCtx.CorrelationId(),
                    orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                    orderUnderAnalysis?.OrderId,
                    null,
                    null,
                    null,
                    noTradesParameters,
                    true,
                    false);

            _judgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }

        protected void SetNoLiveTradesJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(_equitiesParameters);
            var noTradesJudgement =
                new HighProfitJudgement(
                    _ruleCtx.RuleParameterId(),
                    _ruleCtx.CorrelationId(),
                    orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                    orderUnderAnalysis?.OrderId,
                    null,
                    null,
                    null,
                    noTradesParameters,
                    false,
                    true);

            _judgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }
        
        private IExchangeRateProfitBreakdown SetExchangeRateProfits(List<Order> liveTrades)
        {
            var currency = new Domain.Core.Financial.Money.Currency(_equitiesParameters.HighProfitCurrencyConversionTargetCurrency);
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

        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, Domain.Core.Financial.Money.Currency targetCurrency)
        {
            if (!_equitiesParameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                Logger.LogInformation($"GetCostCalculator using non currency conversion one for {targetCurrency.Code} at {UniverseDateTime}");

                return _costCalculatorFactory.CostCalculator();
            }

            Logger.LogInformation($"GetCostCalculator using currency conversion one for {targetCurrency.Code} at {UniverseDateTime}");

            return _costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, Domain.Core.Financial.Money.Currency targetCurrency)
        {
            if (!_equitiesParameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                var calculator =
                    MarketClosureRule
                        ? _revenueCalculatorFactory.RevenueCalculatorMarketClosureCalculator()
                        : _revenueCalculatorFactory.RevenueCalculator();

                Logger.LogInformation($"GetRevenueCalculator using non currency conversion one for {targetCurrency.Code} at {UniverseDateTime}");

                return calculator;
            }
           
            var currencyConvertingCalculator =
                MarketClosureRule
                    ? _revenueCalculatorFactory.RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                    : _revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(targetCurrency);

            Logger.LogInformation($"GetRevenueCalculator using currency conversion one for {targetCurrency.Code} at {UniverseDateTime}");

            return currencyConvertingCalculator;
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
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

            RunRuleForAllDelayedTradingHistoriesInMarket(exchange, UniverseDateTime);
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
