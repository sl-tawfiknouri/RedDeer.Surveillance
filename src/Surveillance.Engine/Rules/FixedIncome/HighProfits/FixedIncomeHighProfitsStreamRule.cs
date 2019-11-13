namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Judgement.FixedIncome;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Factories.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// Analyses fixed income trading for profitability
    /// Considers traded against the market data at the time
    /// To the extent of the time window
    /// </summary>
    public class FixedIncomeHighProfitsStreamRule : BaseUniverseRule, IFixedIncomeHighProfitsStreamRule
    {
        /// <summary>
        /// The parameters for the rule to leverage in analysis
        /// </summary>
        protected readonly IHighProfitsRuleFixedIncomeParameters FixedIncomeParameters;

        /// <summary>
        /// Plumbing code for analysis results aka 'judgements'
        /// </summary>
        protected readonly IFixedIncomeHighProfitJudgementService JudgementService;

        /// <summary>
        /// Auditing context
        /// </summary>
        protected new readonly ISystemProcessOperationRunRuleContext RuleCtx;

        /// <summary>
        /// Logger for rule
        /// </summary>
        protected readonly ILogger<FixedIncomeHighProfitsRule> Logger;

        /// <summary>
        /// Calculate costs including optional leverage of x-rates
        /// </summary>
        private readonly ICostCalculatorFactory costCalculatorFactory;

        /// <summary>
        /// Data requests subscriber for relaying missing data fetch
        /// This pattern is approaching depreciation in favor of proactive
        /// data acquisition
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// Exchange rate profits service
        /// </summary>
        private readonly IExchangeRateProfitCalculator exchangeRateProfitCalculator;

        /// <summary>
        /// Currency conversion service
        /// </summary>
        private readonly ICurrencyConverterService currencyConverterService;

        /// <summary>
        /// Caching strategy - varies over market closure or stream analysis
        /// </summary>
        private readonly IFixedIncomeMarketDataCacheStrategyFactory marketDataCacheFactory;

        /// <summary>
        /// Filters out trades with incorrect CFI
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// Calculates revenues from trading activity
        /// </summary>
        private readonly IRevenueCalculatorFactory revenueCalculatorFactory;

        /// <summary>
        /// Flag for supporting data request subscriber
        /// </summary>
        private bool hasMissingData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitsStreamRule"/> class. 
        /// Constructor for the high profits stream rule
        /// </summary>
        /// <param name="fixedIncomeParameters">
        /// parameters from the client service user interface
        /// </param>
        /// <param name="ruleContext">
        /// auditing helper
        /// </param>
        /// <param name="costCalculatorFactory">
        /// cost logic service factory
        /// </param>
        /// <param name="revenueCalculatorFactory">
        /// revenue logic service factory
        /// </param>
        /// <param name="exchangeRateProfitCalculator">
        /// exchange rate service
        /// </param>
        /// <param name="orderFilter">
        /// classification financial instruments filtering service
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// time bar cache factory
        /// </param>
        /// /// <param name="fixedIncomeMarketCacheFactory">
        /// time bar cache factory
        /// </param>
        /// <param name="marketDataCacheFactory">
        /// market time bar cache factory
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// data fetch pattern helper
        /// </param>
        /// <param name="judgementService">
        /// rule analysis service
        /// </param>
        /// <param name="runMode">
        /// forced or validation
        /// </param>
        /// <param name="logger">
        /// logging helper
        /// </param>
        /// <param name="tradingHistoryLogger">
        /// logging helper for trading history
        /// </param>
        public FixedIncomeHighProfitsStreamRule(
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            ICostCalculatorFactory costCalculatorFactory,
            IRevenueCalculatorFactory revenueCalculatorFactory,
            IExchangeRateProfitCalculator exchangeRateProfitCalculator,
            IUniverseFixedIncomeOrderFilterService orderFilter,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            IFixedIncomeMarketDataCacheStrategyFactory marketDataCacheFactory,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IFixedIncomeHighProfitJudgementService judgementService,
            ICurrencyConverterService currencyService,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighProfitsRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
            : base(
                fixedIncomeParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromHours(8),
                fixedIncomeParameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromHours(8),
                fixedIncomeParameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Domain.Surveillance.Scheduling.Rules.FixedIncomeHighProfits,
                FixedIncomeHighProfitFactory.Version,
                "Fixed Income High Profit Rule",
                ruleContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                runMode,
                logger,
                tradingHistoryLogger)
        {
            this.FixedIncomeParameters =
                fixedIncomeParameters ?? throw new ArgumentNullException(nameof(fixedIncomeParameters));

            this.RuleCtx = ruleContext ?? throw new ArgumentNullException(nameof(ruleContext));

            this.costCalculatorFactory =
                costCalculatorFactory ?? throw new ArgumentNullException(nameof(costCalculatorFactory));

            this.revenueCalculatorFactory = revenueCalculatorFactory
                                             ?? throw new ArgumentNullException(nameof(revenueCalculatorFactory));

            this.marketDataCacheFactory =
                marketDataCacheFactory ?? throw new ArgumentNullException(nameof(marketDataCacheFactory));

            this.exchangeRateProfitCalculator = exchangeRateProfitCalculator
                                                 ?? throw new ArgumentNullException(
                                                     nameof(exchangeRateProfitCalculator));

            this.orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(orderFilter));

            this.dataRequestSubscriber =
                dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));

            this.JudgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
            this.currencyConverterService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));

            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets
        /// Helper support for trade analysis over org factor values such as fund
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// Gets or sets a value indicating whether the rule is a market closure variant
        /// </summary>
        protected bool MarketClosureRule { get; set; } = false;

        /// <summary>
        /// Typed cloning for factor value splitting over values i.e. fund a, fund b
        /// </summary>
        /// <param name="factor">fund a</param>
        /// <returns>a shallow clone of the rule</returns>
        public virtual IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighProfitsStreamRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// Object shallow cloning
        /// </summary>
        /// <returns>shallow clone</returns>
        public object Clone()
        {
            var clone = (FixedIncomeHighProfitsStreamRule)this.MemberwiseClone();
            clone.BaseClone();

            return clone;
        }

        /// <summary>
        /// Handling for order filled events
        /// Not relevant as a trigger for high profits
        /// </summary>
        /// <param name="history">trading history stack</param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// Handling for order filled events with the future window delay applied
        /// Not relevant as a trigger for high profits
        /// </summary>
        /// <param name="history">trading history stack adjusted for future window</param>
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
            if (this.FixedIncomeParameters == null)
            {
                return RuleDataConstraint.Empty().Case;
            }

            var constraints = new List<RuleDataSubConstraint>();

            if (this.FixedIncomeParameters.PerformHighProfitDailyAnalysis)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.RefinitivInterday,
                    _ => !this.orderFilter.Filter(_));

                constraints.Add(constraint);
            }

            if (this.FixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.RefinitivIntraday,
                    _ => !this.orderFilter.Filter(_));

                constraints.Add(constraint);
            }

            return new RuleDataConstraint(
                this.Rule,
                this.FixedIncomeParameters.Id,
                constraints);
        }

        /// <summary>
        /// Handling for the eschaton event
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.Logger.LogInformation("Universe Eschaton occurred.");

            if (!this.MarketClosureRule)
            {
                this.Logger.LogInformation("Running rule for all trading histories at eschaton");
                this.RunRuleForAllTradingHistories();
            }

            if (this.hasMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                this.Logger.LogInformation("Submitting data requests - had missing data and is validation run");
                this.dataRequestSubscriber.SubmitRequest();
                this.RuleCtx?.EndEvent();
            }
            else
            {
                this.Logger.LogInformation("Closing rule analysis");
                this.RuleCtx?.EndEvent();
            }
        }

        /// <summary>
        /// Main method for high profit analysis
        /// </summary>
        /// <param name="history">Trading history qualified for high profit analysis</param>
        /// <param name="intradayCache">Market data for analysis</param>
        protected void EvaluateHighProfits(ITradingHistoryStack history, IUniverseFixedIncomeIntraDayCache intradayCache)
        {
            if (!this.RunRuleGuard(history))
            {
                this.Logger.LogInformation($"EvaluateHighProfits did not pass the rule run guard exiting");

                return;
            }
            
            var orderUnderAnalysis = this.UniverseEvent.UnderlyingEvent as Order;
            var activeTrades = history.ActiveTradeHistory();
            var liveTrades = activeTrades.Where(at => at.OrderStatus() == OrderStatus.Filled).ToList();
            var filteredLiveTrades = this.FilterOutOtc(liveTrades);
            
            this.Logger.LogInformation($"EvaluateHighProfits about to filter over clean / dirty with {filteredLiveTrades.Count} trades");
            var cleanTrades = filteredLiveTrades.Where(_ => _.OrderCleanDirty == OrderCleanDirty.CLEAN).ToList();
            this.Logger.LogInformation($"EvaluateHighProfits filtered by clean and had {cleanTrades.Count} trades");

            if (orderUnderAnalysis == null)
            {
                orderUnderAnalysis = activeTrades.LastOrDefault();
            }

            if (!cleanTrades.Any())
            {
                this.Logger.LogInformation($"EvaluateHighProfits had no active and filled trades, exiting");
                this.SetNoLiveTradesJudgement(orderUnderAnalysis);

                return;
            }

            var targetCurrency = new Currency(this.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency);
            var allTradesInCommonCurrency = this.CheckTradesInCommonCurrency(cleanTrades, targetCurrency);
            var costCalculator = this.GetCostCalculator(allTradesInCommonCurrency, targetCurrency);
            var revenueCalculator = this.GetRevenueCalculator(allTradesInCommonCurrency, targetCurrency);

            var marketCache = 
                this.MarketClosureRule
                    ? this.marketDataCacheFactory.InterdayStrategy(this.UniverseFixedIncomeInterdayCache)
                    : this.marketDataCacheFactory.IntradayStrategy(intradayCache);

            var costTask = costCalculator.CalculateCostOfPosition(cleanTrades, this.UniverseDateTime, this.RuleCtx);
            var revenueTask = revenueCalculator.CalculateRevenueOfPosition(cleanTrades, this.UniverseDateTime, this.RuleCtx, marketCache);

            var cost = costTask.Result;
            var revenueResponse = revenueTask.Result;

            if (revenueResponse.HadMissingMarketData)
            {
                this.Logger.LogInformation($"Had missing market data for fixed income high profits, exiting {cleanTrades.FirstOrDefault()?.Instrument?.Identifiers}");
                this.SetMissingMarketDataJudgement(orderUnderAnalysis);
                this.hasMissingData = true;

                return;
            }

            var revenue = revenueResponse.Money;

            if (revenue == null || revenue.Value.Value <= 0)
            {
                this.Logger.LogInformation($"rule had null for revenues for {cleanTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Returning.");
                this.NoRevenueOrCostJudgement(orderUnderAnalysis);

                return;
            }

            if (cost == null || cost.Value.Value <= 0)
            {
                this.Logger.LogInformation(
                    $"We have calculable revenues but not costs for {cleanTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Returning.");
                this.NoRevenueOrCostJudgement(orderUnderAnalysis);

                return;
            }

            if (!revenue.Value.DenominatedInCommonCurrency(cost.Value))
            {
                var convertedCostTask =
                    this.currencyConverterService.Convert(
                        new[] { cost.Value },
                        revenue.Value.Currency,
                        UniverseDateTime,
                        this.RuleCtx);

                var convertedCost = convertedCostTask.Result;

                if (convertedCost == null)
                {
                    this.Logger.LogError($"Currency of revenue {revenue.Value.Currency} - currency of costs {cost.Value.Currency} for trade {liveTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. Could not convert cost to revenue.");

                    return;
                }

                cost = convertedCost;
            }

            var absoluteProfit = revenue.Value - cost.Value;
            var profitRatio = (revenue.Value.Value / cost.Value.Value) - 1;

            var hasHighProfitAbsolute = this.HasHighProfitAbsolute(absoluteProfit);
            var hasHighProfitPercentage = this.HasHighProfitPercentage(profitRatio);

            IExchangeRateProfitBreakdown exchangeRateProfits = null;

            if (this.FixedIncomeParameters.UseCurrencyConversions
                && !string.IsNullOrEmpty(this.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency))
            {
                this.Logger.LogInformation(
                    $"is set to use currency conversions and has a target conversion currency to {this.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency}. Calling set exchange rate profits.");

                exchangeRateProfits = this.SetExchangeRateProfits(cleanTrades);
            }

            RuleBreachContext ruleBreachContext = null;

            if (hasHighProfitAbsolute || hasHighProfitPercentage)
            {
                this.Logger.LogInformation(
                    $"had a breach for {cleanTrades.FirstOrDefault()?.Instrument?.Identifiers} at {this.UniverseDateTime}. High Profit Absolute {hasHighProfitAbsolute} and High Profit Percentage {hasHighProfitPercentage}.");

                ruleBreachContext = new RuleBreachContext(
                    this.FixedIncomeParameters.Windows.BackwardWindowSize
                    + this.FixedIncomeParameters.Windows.FutureWindowSize,
                    new TradePosition(cleanTrades),
                    cleanTrades.FirstOrDefault(_ => _?.Instrument != null)?.Instrument,
                    this.RuleCtx.IsBackTest(),
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.SystemProcessOperationContext().Id.ToString(),
                    this.RuleCtx.CorrelationId(),
                    this.OrganisationFactorValue,
                    this.FixedIncomeParameters,
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
        /// The universe event to check filtering for.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The genesis event override.
        /// </summary>
        protected override void Genesis()
        {
            this.Logger.LogInformation("Universe Genesis occurred");
        }

        /// <summary>
        /// The market close triggered event method.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.Logger.LogInformation($"Trading closed for exchange {exchange.MarketId}. Running market closure virtual profits check.");

            this.RunRuleForAllDelayedTradingHistoriesInMarket(exchange, this.UniverseDateTime);
        }

        /// <summary>
        /// The market open triggered event method.
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
        /// The run initial submission event delayed for future window offset.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event triggered method.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.UniverseFixedIncomeIntradayCache);
        }

        /// <summary>
        /// The run post order event delayed triggered method with future window delay offset.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            this.EvaluateHighProfits(history, this.FutureUniverseFixedIncomeIntradayCache);
        }

        /// <summary>
        /// The run rule guard, always true but provides override to derived classes.
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
        /// The set no live trades judgement for when we have no filled trades.
        /// Performs all downstream side effect for the trade in its logical branch
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        protected void SetNoLiveTradesJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this.FixedIncomeParameters);
            var noTradesJudgement = new FixedIncomeHighProfitJudgement(
                this.RuleCtx.RuleParameterId(),
                this.RuleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                false,
                true);

            this.JudgementService.Judgement(new FixedIncomeHighProfitJudgementContext(noTradesJudgement, false));
        }

        /// <summary>
        /// The filter out over the counter trades.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<Order> FilterOutOtc(IReadOnlyCollection<Order> orders)
        {
            if (orders == null || !orders.Any())
            {
                return new List<Order>();
            }

            return 
                orders
                    .Where(_ => _.Market?.Type != MarketTypes.OTC)
                    .Where(_ => _.OrderType != OrderTypes.OTC)
                    .ToList();
        }

        /// <summary>
        /// The check trades in common currency method provides preliminary calculations for calculators.
        /// </summary>
        /// <param name="liveTrades">
        /// The live trades.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckTradesInCommonCurrency(IReadOnlyCollection<Order> liveTrades, Currency targetCurrency)
        {
            return
                liveTrades.Any()
                && liveTrades.All(x =>
                    string.Equals(
                        x.OrderCurrency.Code,
                        targetCurrency.Code,
                        StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// The get cost calculator call with a switch for parameters in play.
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
            if (!this.FixedIncomeParameters.UseCurrencyConversions
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                this.Logger.LogInformation(
                    $"GetCostCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return this.costCalculatorFactory.CostCalculator();
            }

            this.Logger.LogInformation(
                $"GetCostCalculator using currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

            return this.costCalculatorFactory.CurrencyConvertingCalculator(targetCurrency);
        }

        /// <summary>
        /// The get revenue calculator with a switch for parameters in play i.e. currency conversions.
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
            if (!this.FixedIncomeParameters.UseCurrencyConversions 
                || allTradesInCommonCurrency
                || string.IsNullOrWhiteSpace(targetCurrency.Code))
            {
                var calculator = 
                    this.MarketClosureRule
                     ? this.revenueCalculatorFactory.RevenueCalculatorMarketClosureCalculator()
                     : this.revenueCalculatorFactory.RevenueCalculator();

                this.Logger.LogInformation(
                    $"GetRevenueCalculator using non currency conversion one for {targetCurrency.Code} at {this.UniverseDateTime}");

                return calculator;
            }

            var currencyConvertingCalculator = 
                this.MarketClosureRule
                    ? this.revenueCalculatorFactory.RevenueCurrencyConvertingMarketClosureCalculator(targetCurrency)
                    : this.revenueCalculatorFactory.RevenueCurrencyConvertingCalculator(targetCurrency);

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
        /// Exceptional scenario if absolute profits don't match over currencies with the target currency
        /// </exception>
        private bool HasHighProfitAbsolute(Money absoluteProfits)
        {
            if (this.FixedIncomeParameters.HighProfitAbsoluteThreshold == null)
            {
                return false;
            }

            if (this.FixedIncomeParameters.UseCurrencyConversions
                && !string.Equals(
                    this.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency,
                    absoluteProfits.Currency.Code,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                this.RuleCtx.EventException("had mismatching absolute profits currencies.");

                throw new InvalidOperationException("had mismatching absolute profits currencies.");
            }

            return absoluteProfits.Value >= this.FixedIncomeParameters.HighProfitAbsoluteThreshold;
        }

        /// <summary>
        /// The has high profit percentage check for percentage profits and profit ratios.
        /// </summary>
        /// <param name="profitRatio">
        /// The profit ratio.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasHighProfitPercentage(decimal profitRatio)
        {
            return this.FixedIncomeParameters.HighProfitPercentageThreshold.HasValue
                   && this.FixedIncomeParameters.HighProfitPercentageThreshold.Value <= profitRatio;
        }

        /// <summary>
        /// The no revenue or cost judgement performs final work and calls downstream
        /// side effects for this logic branch.
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        private void NoRevenueOrCostJudgement(Order orderUnderAnalysis)
        {
            var noRevenueJsonParameters = JsonConvert.SerializeObject(this.FixedIncomeParameters);

            var noRevenueJudgement = new FixedIncomeHighProfitJudgement(
                this.RuleCtx.RuleParameterId(),
                this.RuleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noRevenueJsonParameters,
                false,
                false);

            this.JudgementService.Judgement(new FixedIncomeHighProfitJudgementContext(noRevenueJudgement, false));
        }

        /// <summary>
        /// The set exchange rate profits method which will figure out the break down of profits
        /// due to currency movements vs trading activity.
        /// </summary>
        /// <param name="liveTrades">
        /// The live trades.
        /// </param>
        /// <returns>
        /// The <see cref="IExchangeRateProfitBreakdown"/>.
        /// </returns>
        private IExchangeRateProfitBreakdown SetExchangeRateProfits(IReadOnlyCollection<Order> liveTrades)
        {
            var currency = new Currency(this.FixedIncomeParameters.HighProfitCurrencyConversionTargetCurrency);
            var buys = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.BUY || lt.OrderDirection == OrderDirections.COVER)
                    .ToList());

            var sells = new TradePosition(
                liveTrades.Where(
                        lt => lt.OrderDirection == OrderDirections.SELL || lt.OrderDirection == OrderDirections.SHORT)
                    .ToList());

            var exchangeRateProfitsTask = this.exchangeRateProfitCalculator.ExchangeRateMovement(
                buys,
                sells,
                currency,
                this.RuleCtx);

            var exchangeRateProfits = exchangeRateProfitsTask.Result;

            return exchangeRateProfits;
        }

        /// <summary>
        /// The set judgement for full analysis covers downstream logic for its code branch.
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

            var jsonParameters = JsonConvert.SerializeObject(this.FixedIncomeParameters);
            var judgement = new FixedIncomeHighProfitJudgement(
                this.RuleCtx.RuleParameterId(),
                this.RuleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                absoluteHighProfit,
                absoluteHighProfitCurrency,
                percentageHighProfit,
                jsonParameters,
                false,
                false);

            this.JudgementService.Judgement(
                new FixedIncomeHighProfitJudgementContext(
                    judgement,
                    hasHighProfitAbsolute || hasHighProfitPercentage,
                    ruleBreachContext,
                    this.FixedIncomeParameters,
                    absoluteProfit,
                    absoluteProfit.Currency.Symbol,
                    profitRatio,
                    hasHighProfitAbsolute,
                    hasHighProfitPercentage,
                    exchangeRateProfits));
        }

        /// <summary>
        /// The set missing market data judgement covers downstream processing logic for its logical code flow branch.
        /// </summary>
        /// <param name="orderUnderAnalysis">
        /// The order under analysis.
        /// </param>
        private void SetMissingMarketDataJudgement(Order orderUnderAnalysis)
        {
            var noTradesParameters = JsonConvert.SerializeObject(this.FixedIncomeParameters);
            var noTradesJudgement = new FixedIncomeHighProfitJudgement(
                this.RuleCtx.RuleParameterId(),
                this.RuleCtx.CorrelationId(),
                orderUnderAnalysis?.ReddeerOrderId?.ToString(),
                orderUnderAnalysis?.OrderId,
                null,
                null,
                null,
                noTradesParameters,
                true,
                false);

            this.JudgementService.Judgement(new FixedIncomeHighProfitJudgementContext(noTradesJudgement, false));
        }
    }
}
