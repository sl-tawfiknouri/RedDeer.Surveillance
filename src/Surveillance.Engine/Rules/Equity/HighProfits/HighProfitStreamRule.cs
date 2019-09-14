using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Judgement.Equity;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        protected readonly IHighProfitsRuleEquitiesParameters _equitiesParameters;

        protected readonly IHighProfitJudgementService _judgementService;

        protected readonly ISystemProcessOperationRunRuleContext _ruleCtx;

        protected readonly ILogger<HighProfitsRule> Logger;

        protected bool MarketClosureRule = false;

        private readonly ICostCalculatorFactory _costCalculatorFactory;

        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly IExchangeRateProfitCalculator _exchangeRateProfitCalculator;

        private readonly IMarketDataCacheStrategyFactory _marketDataCacheFactory;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly IRevenueCalculatorFactory _revenueCalculatorFactory;

        private bool _hasMissingData;

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
            IHighProfitJudgementService judgementService,
            RuleRunMode runMode,
            ILogger<HighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromHours(8),
                equitiesParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromHours(8),
                equitiesParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.HighProfits,
                EquityRuleHighProfitFactory.Version,
                "High Profit Rule",
                ruleCtx,
                marketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this._equitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this._ruleCtx = ruleCtx ?? throw new ArgumentNullException(nameof(ruleCtx));
            this._costCalculatorFactory =
                costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            this._revenueCalculatorFactory = revenueCalculatorFactory
                                             ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            this._marketDataCacheFactory =
                marketDataCacheFactory ?? throw new ArgumentNullException(nameof(marketDataCacheFactory));
            this._exchangeRateProfitCalculator = exchangeRateProfitCalculator
                                                 ?? throw new ArgumentNullException(
                                                     nameof(exchangeRateProfitCalculator));
            this._orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this._dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this._judgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        public virtual IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighProfitStreamRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            var clone = (HighProfitStreamRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this.Logger.LogInformation("Universe Eschaton occurred.");

            if (!this.MarketClosureRule) this.RunRuleForAllTradingHistories();

            if (this._hasMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                this.Logger.LogInformation("Deleting alerts off the message sender");
                this._dataRequestSubscriber.SubmitRequest();
                this._ruleCtx?.EndEvent();
            }
            else
            {
                this._ruleCtx?.EndEvent();
            }
        }

        protected void EvaluateHighProfits(ITradingHistoryStack history, IUniverseEquityIntradayCache intradayCache)
        {
            if (!this.RunRuleGuard(history)) return;

            var orderUnderAnalysis = this.UniverseEvent.UnderlyingEvent as Order;

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades.Where(at => at.OrderStatus() == OrderStatus.Filled).ToList();

            if (orderUnderAnalysis == null) orderUnderAnalysis = activeTrades.LastOrDefault();

            if (!liveTrades.Any())
            {
                this.SetNoLiveTradesJudgement(orderUnderAnalysis);

                return;
            }

            var targetCurrency = new Currency(this._equitiesParameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency = liveTrades.Any() && liveTrades.All(
                                                x => string.Equals(
                                                    x.OrderCurrency.Code,
                                                    targetCurrency.Code,
                                                    StringComparison.InvariantCultureIgnoreCase));

            var costCalculator = this.GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = this.GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var marketCache = this.MarketClosureRule
                                  ? this._marketDataCacheFactory.InterdayStrategy(this.UniverseEquityInterdayCache)
                                  : this._marketDataCacheFactory.IntradayStrategy(intradayCache);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, this.UniverseDateTime, this._ruleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(
                liveTrades,
                this.UniverseDateTime,
                this._ruleCtx,
                marketCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                this.SetMissingMarketDataJudgement(orderUnderAnalysis);
                this._hasMissingData = true;

                return;
            }

            var revenue = revenueResponse.Money;

            if (revenue == null || revenue.Value.Value <= 0)
            {
                this.Logger.LogInformation(
                    $"rule had null for revenues for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Returning.");

                this.NoRevenueOrCostJudgement(orderUnderAnalysis);
                return;
            }

            if (cost == null || cost.Value.Value <= 0)
            {
                this.Logger.LogError(
                    $"something went wrong. We have calculable revenues but not costs for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Returning.");

                this.NoRevenueOrCostJudgement(orderUnderAnalysis);
                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = revenue.Value.Value / cost.Value.Value - 1;

            var hasHighProfitAbsolute = this.HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = this.HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (this._equitiesParameters.UseCurrencyConversions && !string.IsNullOrEmpty(
                    this._equitiesParameters.HighProfitCurrencyConversionTargetCurrency))
            {
                this.Logger.LogInformation(
                    $"is set to use currency conversions and has a target conversion currency to {this._equitiesParameters.HighProfitCurrencyConversionTargetCurrency}. Calling set exchange rate profits.");

                exchangeRateProfits = this.SetExchangeRateProfits(liveTrades);
            }

            RuleBreachContext ruleBreachContext = null;

            if (hasHighProfitAbsolute || hasHighProfitPercentage)
            {
                this.Logger.LogInformation(
                    $"had a breach for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

                ruleBreachContext = new RuleBreachContext(
                    this._equitiesParameters.Windows.BackwardWindowSize
                    + this._equitiesParameters.Windows.FutureWindowSize,
                    new TradePosition(liveTrades),
                    liveTrades.FirstOrDefault(_ => _?.Instrument != null)?.Instrument,
                    this._ruleCtx.IsBackTest(),
                    this._ruleCtx.RuleParameterId(),
                    this._ruleCtx.SystemProcessOperationContext().Id.ToString(),
                    this._ruleCtx.CorrelationId(),
                    this.OrganisationFactorValue,
                    this._equitiesParameters,
                    this.UniverseDateTime);
            }

            this.SetJudgementForFullAnalysis(
                absoluteProfit,
                profitRatio,
                hasHighProfitAbsolute,
                hasHighProfitPercentage,
                exchangeRateProfits,
                ruleBreachContext,
                orderUnderAnalysis);
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this.Logger.LogInformation("Universe Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.Logger.LogInformation(
                $"Trading closed for exchange {exchange.MarketId}. Running market closure virtual profits check.");

            this.RunRuleForAllDelayedTradingHistoriesInMarket(exchange, this.UniverseDateTime);
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.Logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.UniverseEquityIntradayCache);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.FutureUniverseEquityIntradayCache);
        }

        protected virtual bool RunRuleGuard(ITradingHistoryStack history)
        {
            return true;
        }

        protected void SetNoLiveTradesJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this._equitiesParameters);
            var noTradesJudgement = new HighProfitJudgement(
                this._ruleCtx.RuleParameterId(),
                this._ruleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                false,
                true);

            this._judgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }

        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, Currency targetCurrency)
        {
            if (!this._equitiesParameters.UseCurrencyConversions || allTradesInCommonCurrency
                                                                 || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                this.Logger.LogInformation(
                    $"GetCostCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return this._costCalculatorFactory.CostCalculator();
            }

            this.Logger.LogInformation(
                $"GetCostCalculator using currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

            return this._costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, Currency targetCurrency)
        {
            if (!this._equitiesParameters.UseCurrencyConversions || allTradesInCommonCurrency
                                                                 || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                var calculator = this.MarketClosureRule
                                     ? this._revenueCalculatorFactory.RevenueCalculatorMarketClosureCalculator()
                                     : this._revenueCalculatorFactory.RevenueCalculator();

                this.Logger.LogInformation(
                    $"GetRevenueCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return calculator;
            }

            var currencyConvertingCalculator = this.MarketClosureRule
                                                   ? this._revenueCalculatorFactory
                                                       .RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                                                   : this._revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(
                                                       targetCurrency);

            this.Logger.LogInformation(
                $"GetRevenueCalculator using currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

            return currencyConvertingCalculator;
        }

        private bool HasHighProfitAbsolute(Money absoluteProfits)
        {
            if (this._equitiesParameters.HighProfitAbsoluteThreshold == null) return false;

            if (this._equitiesParameters.UseCurrencyConversions && !string.Equals(
                    this._equitiesParameters.HighProfitCurrencyConversionTargetCurrency,
                    absoluteProfits.Currency.Code,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                this._ruleCtx.EventException(
                    "had mismatching absolute profits currencies. Something went horribly wrong!");
                throw new InvalidOperationException(
                    "had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= this._equitiesParameters.HighProfitAbsoluteThreshold;
        }

        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return this._equitiesParameters.HighProfitPercentageThreshold.HasValue
                   && this._equitiesParameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        private void NoRevenueOrCostJudgement(Order orderUnderAnalysis)
        {
            var noRevenueJsonParameters = JsonConvert.SerializeObject(this._equitiesParameters);
            var noRevenueJudgement = new HighProfitJudgement(
                this._ruleCtx.RuleParameterId(),
                this._ruleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noRevenueJsonParameters,
                false,
                false);

            this._judgementService.Judgement(new HighProfitJudgementContext(noRevenueJudgement, false));
        }

        private IExchangeRateProfitBreakdown SetExchangeRateProfits(List<Order> liveTrades)
        {
            var currency = new Currency(this._equitiesParameters.HighProfitCurrencyConversionTargetCurrency);
            var buys = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.BUY || lt.OrderDirection == OrderDirections.COVER)
                    .ToList());

            var sells = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.SELL || lt.OrderDirection == OrderDirections.SHORT)
                    .ToList());

            var exchangeRateProfitsTask = this._exchangeRateProfitCalculator.ExchangeRateMovement(
                buys,
                sells,
                currency,
                this._ruleCtx);

            var exchangeRateProfits = exchangeRateProfitsTask.Result;

            return exchangeRateProfits;
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
            var absoluteHighProfit = hasHighProfitAbsolute ? absoluteProfit.Value : (decimal?)null;

            var absoluteHighProfitCurrency = hasHighProfitAbsolute ? absoluteProfit.Currency.Code : null;

            var percentageHighProfit = hasHighProfitPercentage ? profitRatio : (decimal?)null;

            var jsonParameters = JsonConvert.SerializeObject(this._equitiesParameters);
            var judgement = new HighProfitJudgement(
                this._ruleCtx.RuleParameterId(),
                this._ruleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                absoluteHighProfit,
                absoluteHighProfitCurrency,
                percentageHighProfit,
                jsonParameters,
                false,
                false);

            this._judgementService.Judgement(
                new HighProfitJudgementContext(
                    judgement,
                    hasHighProfitAbsolute || hasHighProfitPercentage,
                    ruleBreachContext,
                    this._equitiesParameters,
                    absoluteProfit,
                    absoluteProfit.Currency.Symbol,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits));
        }

        private void SetMissingMarketDataJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this._equitiesParameters);
            var noTradesJudgement = new HighProfitJudgement(
                this._ruleCtx.RuleParameterId(),
                this._ruleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                true,
                false);

            this._judgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }
    }
}