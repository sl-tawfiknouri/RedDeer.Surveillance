namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Judgement.Equity;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The high profit stream rule.
    /// </summary>
    public class HighProfitStreamRule : BaseUniverseRule, IHighProfitStreamRule
    {
        /// <summary>
        /// The equities parameters.
        /// </summary>
        protected readonly IHighProfitsRuleEquitiesParameters EquitiesParameters;

        /// <summary>
        /// The judgement service.
        /// </summary>
        protected readonly IHighProfitJudgementService JudgementService;

        /// <summary>
        /// The rule context.
        /// </summary>
        protected readonly ISystemProcessOperationRunRuleContext RuleContext;

        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger<HighProfitsRule> Logger;

        /// <summary>
        /// The market closure rule.
        /// </summary>
        protected bool MarketClosureRule = false;

        /// <summary>
        /// The cost calculator factory.
        /// </summary>
        private readonly ICostCalculatorFactory CostCalculatorFactory;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber DataRequestSubscriber;

        /// <summary>
        /// The exchange rate profit calculator.
        /// </summary>
        private readonly IExchangeRateProfitCalculator ExchangeRateProfitCalculator;

        /// <summary>
        /// The market data cache factory.
        /// </summary>
        private readonly IEquityMarketDataCacheStrategyFactory MarketDataCacheFactory;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter OrderFilter;

        /// <summary>
        /// The revenue calculator factory.
        /// </summary>
        private readonly IRevenueCalculatorFactory RevenueCalculatorFactory;

        /// <summary>
        /// The has missing data.
        /// </summary>
        private bool HasMissingData;


        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitStreamRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="costCalculatorFactory">
        /// The cost calculator factory.
        /// </param>
        /// <param name="revenueCalculatorFactory">
        /// The revenue calculator factory.
        /// </param>
        /// <param name="exchangeRateProfitCalculator">
        /// The exchange rate profit calculator.
        /// </param>
        /// <param name="orderFilter">
        /// The order filter.
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// /// <param name="fixedIncomeMarketCacheFactory">
        /// The market cache factory.
        /// </param>
        /// <param name="marketDataCacheFactory">
        /// The market data cache factory.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// The trading history logger.
        /// </param>
        public HighProfitStreamRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseOrderFilter orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IEquityMarketDataCacheStrategyFactory marketDataCacheFactory,
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
                ruleContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.EquitiesParameters =
                equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.RuleContext = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));
            this.CostCalculatorFactory =
                costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));
            this.RevenueCalculatorFactory = 
                revenueCalculatorFactory ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));
            this.MarketDataCacheFactory =
                marketDataCacheFactory ?? throw new ArgumentNullException(nameof(marketDataCacheFactory));
            this.ExchangeRateProfitCalculator =
                exchangeRateProfitCalculator ?? throw new ArgumentNullException(nameof(exchangeRateProfitCalculator));
            this.OrderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));
            this.DataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.JudgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organisation factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public virtual IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (HighProfitStreamRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var clone = (HighProfitStreamRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            if (this.EquitiesParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraints = new List<RuleDataSubConstraint>();

            if (this.EquitiesParameters.PerformHighProfitDailyAnalysis)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => !this.OrderFilter.Filter(_));

                constraints.Add(constraint);
            }

            if (this.EquitiesParameters.PerformHighProfitWindowAnalysis)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyIntraday,
                    _ => !this.OrderFilter.Filter(_));

                constraints.Add(constraint);
            }

            return new RuleDataConstraint(
                this.Rule,
                this.EquitiesParameters.Id,
                constraints);
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.Logger.LogInformation("Universe Eschaton occurred.");

            if (!this.MarketClosureRule)
            {
                this.RunRuleForAllTradingHistories();
            }

            if (this.HasMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                this.Logger.LogInformation("Deleting alerts off the message sender");
                this.DataRequestSubscriber.SubmitRequest();
                this.RuleContext?.EndEvent();
            }
            else
            {
                this.RuleContext?.EndEvent();
            }
        }

        /// <summary>
        /// The evaluate high profits.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        /// <param name="intradayCache">
        /// The intraday cache.
        /// </param>
        protected void EvaluateHighProfits(ITradingHistoryStack history, IUniverseEquityIntraDayCache intradayCache)
        {
            if (!this.RunRuleGuard(history))
            {
                return;
            }

            var orderUnderAnalysis = this.UniverseEvent.UnderlyingEvent as Order;

            var activeTrades = history.ActiveTradeHistory();

            var liveTrades = activeTrades.Where(at => at.OrderStatus() == OrderStatus.Filled).ToList();

            if (orderUnderAnalysis == null)
            {
                orderUnderAnalysis = activeTrades.LastOrDefault();
            }

            if (!liveTrades.Any())
            {
                this.SetNoLiveTradesJudgement(orderUnderAnalysis);

                return;
            }

            var targetCurrency = new Currency(this.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency);

            var allTradesInCommonCurrency = liveTrades.Any() && liveTrades.All(
                                                x => string.Equals(
                                                    x.OrderCurrency.Code,
                                                    targetCurrency.Code,
                                                    StringComparison.InvariantCultureIgnoreCase));

            var costCalculator = this.GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = this.GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var marketCache = this.MarketClosureRule
                                  ? this.MarketDataCacheFactory.InterdayStrategy(this.UniverseEquityInterdayCache)
                                  : this.MarketDataCacheFactory.IntradayStrategy(intradayCache);

            var costTask = costCalculator.CalculateCostOfPosition(liveTrades, this.UniverseDateTime, this.RuleContext);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(
                liveTrades,
                this.UniverseDateTime,
                this.RuleContext,
                marketCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                this.SetMissingMarketDataJudgement(orderUnderAnalysis);
                this.HasMissingData = true;

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

            if (!revenue.Value.DenominatedInCommonCurrency(cost.Value))
            {
                this.Logger.LogError($"Currency of revenue {revenue.Value.Currency} - currency of costs {cost.Value.Currency} for trade {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}.");

                return;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = revenue.Value.Value / cost.Value.Value - 1;

            var hasHighProfitAbsolute = this.HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = this.HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (this.EquitiesParameters.UseCurrencyConversions && !string.IsNullOrEmpty(
                    this.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency))
            {
                this.Logger.LogInformation(
                    $"is set to use currency conversions and has a target conversion currency to {this.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency}. Calling set exchange rate profits.");

                exchangeRateProfits = this.SetExchangeRateProfits(liveTrades);
            }

            RuleBreachContext ruleBreachContext = null;

            if (hasHighProfitAbsolute || hasHighProfitPercentage)
            {
                this.Logger.LogInformation(
                    $"had a breach for {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

                ruleBreachContext = new RuleBreachContext(
                    this.EquitiesParameters.Windows.BackwardWindowSize
                    + this.EquitiesParameters.Windows.FutureWindowSize,
                    new TradePosition(liveTrades),
                    liveTrades.FirstOrDefault(_ => _?.Instrument != null)?.Instrument,
                    this.RuleContext.IsBackTest(),
                    this.RuleContext.RuleParameterId(),
                    this.RuleContext.SystemProcessOperationContext().Id.ToString(),
                    this.RuleContext.CorrelationId(),
                    this.OrganisationFactorValue,
                    this.EquitiesParameters,
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

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.OrderFilter.Filter(value);
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.Logger.LogInformation("Universe Genesis occurred");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.Logger.LogInformation(
                $"Trading closed for exchange {exchange.MarketId}. Running market closure virtual profits check.");

            this.RunRuleForAllDelayedTradingHistoriesInMarket(exchange, this.UniverseDateTime);
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.Logger.LogInformation($"Trading Opened for exchange {exchange.MarketId}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.UniverseEquityIntradayCache);
        }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.FutureUniverseEquityIntradayCache);
        }

        /// <summary>
        /// The run rule guard.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool RunRuleGuard(ITradingHistoryStack history)
        {
            return true;
        }

        /// <summary>
        /// The set no live trades judgement.
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        protected void SetNoLiveTradesJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this.EquitiesParameters);
            var noTradesJudgement = new HighProfitJudgement(
                this.RuleContext.RuleParameterId(),
                this.RuleContext.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                false,
                true);

            this.JudgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }

        /// <summary>
        /// The get cost calculator.
        /// </summary>
        /// <param name="allTradesInCommonCurrency">
        /// The all trades in common currency.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="ICostCalculator"/>.
        /// </returns>
        private ICostCalculator GetCostCalculator(bool allTradesInCommonCurrency, Currency targetCurrency)
        {
            if (!this.EquitiesParameters.UseCurrencyConversions || allTradesInCommonCurrency
                                                                 || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                this.Logger.LogInformation(
                    $"GetCostCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return this.CostCalculatorFactory.CostCalculator();
            }

            this.Logger.LogInformation(
                $"GetCostCalculator using currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

            return this.CostCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        /// <summary>
        /// The get revenue calculator.
        /// </summary>
        /// <param name="allTradesInCommonCurrency">
        /// The all trades in common currency.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="IRevenueCalculator"/>.
        /// </returns>
        private IRevenueCalculator GetRevenueCalculator(bool allTradesInCommonCurrency, Currency targetCurrency)
        {
            if (!this.EquitiesParameters.UseCurrencyConversions || allTradesInCommonCurrency
                                                                 || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                var calculator = this.MarketClosureRule
                                     ? this.RevenueCalculatorFactory.RevenueCalculatorMarketClosureCalculator()
                                     : this.RevenueCalculatorFactory.RevenueCalculator();

                this.Logger.LogInformation(
                    $"GetRevenueCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return calculator;
            }

            var currencyConvertingCalculator = this.MarketClosureRule
                                                   ? this.RevenueCalculatorFactory
                                                       .RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                                                   : this.RevenueCalculatorFactory.RevenueCurrencyConvertingCalculator(
                                                       targetCurrency);

            this.Logger.LogInformation(
                $"GetRevenueCalculator using currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

            return currencyConvertingCalculator;
        }

        /// <summary>
        /// The has high profit absolute.
        /// </summary>
        /// <param name="absoluteProfits">
        /// The absolute profits.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private bool HasHighProfitAbsolute(Money absoluteProfits)
        {
            if (this.EquitiesParameters.HighProfitAbsoluteThreshold == null) return false;

            if (this.EquitiesParameters.UseCurrencyConversions && !string.Equals(
                    this.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency,
                    absoluteProfits.Currency.Code,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                this.RuleContext.EventException(
                    "had mismatching absolute profits currencies. Something went horribly wrong!");
                throw new InvalidOperationException(
                    "had mismatching absolute profits currencies. Something went horribly wrong!");
            }

            return absoluteProfits.Value >= this.EquitiesParameters.HighProfitAbsoluteThreshold;
        }

        /// <summary>
        /// The has high profit percentage.
        /// </summary>
        /// <param name="profitRatio">
        /// The profit ratio.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return this.EquitiesParameters.HighProfitPercentageThreshold.HasValue
                   && this.EquitiesParameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        /// <summary>
        /// The no revenue or cost judgement.
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        private void NoRevenueOrCostJudgement(Order orderUnderAnalysis)
        {
            var noRevenueJsonParameters = JsonConvert.SerializeObject(this.EquitiesParameters);
            var noRevenueJudgement = new HighProfitJudgement(
                this.RuleContext.RuleParameterId(),
                this.RuleContext.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noRevenueJsonParameters,
                false,
                false);

            this.JudgementService.Judgement(new HighProfitJudgementContext(noRevenueJudgement, false));
        }

        /// <summary>
        /// The set exchange rate profits.
        /// </summary>
        /// <param name="liveTrades">
        /// The live trades.
        /// </param>
        /// <returns>
        /// The <see cref="IExchangeRateProfitBreakdown"/>.
        /// </returns>
        private IExchangeRateProfitBreakdown SetExchangeRateProfits(List<Order> liveTrades)
        {
            var currency = new Currency(this.EquitiesParameters.HighProfitCurrencyConversionTargetCurrency);
            var buys = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.BUY || lt.OrderDirection == OrderDirections.COVER)
                    .ToList());

            var sells = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.SELL || lt.OrderDirection == OrderDirections.SHORT)
                    .ToList());

            var exchangeRateProfitsTask = this.ExchangeRateProfitCalculator.ExchangeRateMovement(
                buys,
                sells,
                currency,
                this.RuleContext);

            var exchangeRateProfits = exchangeRateProfitsTask.Result;

            return exchangeRateProfits;
        }

        /// <summary>
        /// The set judgement for full analysis.
        /// </summary>
        /// <param name="absoluteProfit">
        /// The absolute profit.
        /// </param>
        /// <param name="profitRatio">
        /// The profit ratio.
        /// </param>
        /// <param name="hasHighProfitAbsolute">
        /// The has high profit absolute.
        /// </param>
        /// <param name="hasHighProfitPercentage">
        /// The has high profit percentage.
        /// </param>
        /// <param name="exchangeRateProfits">
        /// The exchange rate profits.
        /// </param>
        /// <param name="ruleBreachContext">
        /// The rule breach context.
        /// </param>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
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

            var jsonParameters = JsonConvert.SerializeObject(this.EquitiesParameters);
            var judgement = new HighProfitJudgement(
                this.RuleContext.RuleParameterId(),
                this.RuleContext.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                absoluteHighProfit,
                absoluteHighProfitCurrency,
                percentageHighProfit,
                jsonParameters,
                false,
                false);

            this.JudgementService.Judgement(
                new HighProfitJudgementContext(
                    judgement,
                    hasHighProfitAbsolute || hasHighProfitPercentage,
                    ruleBreachContext,
                    this.EquitiesParameters,
                    absoluteProfit,
                    absoluteProfit.Currency.Symbol,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits));
        }

        /// <summary>
        /// The set missing market data judgement.
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        private void SetMissingMarketDataJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this.EquitiesParameters);
            var noTradesJudgement = new HighProfitJudgement(
                this.RuleContext.RuleParameterId(),
                this.RuleContext.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                true,
                false);

            this.JudgementService.Judgement(new HighProfitJudgementContext(noTradesJudgement, false));
        }
    }
}