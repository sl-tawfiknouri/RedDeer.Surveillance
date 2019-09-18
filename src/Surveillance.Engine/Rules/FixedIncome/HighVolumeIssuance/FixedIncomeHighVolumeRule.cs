namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance
{
    using System;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Judgement.FixedIncome;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    /// <summary>
    /// The fixed income high volume issuance rule.
    /// </summary>
    public class FixedIncomeHighVolumeRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        // potentially useful?
        private readonly IMarketTradingHoursService tradingHoursService;
        private readonly ICurrencyConverterService currencyConverterService;

        /// <summary>
        /// The order filter service for CFI codes.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The rule run parameters.
        /// </summary>
        private readonly IHighVolumeIssuanceRuleFixedIncomeParameters parameters;

        /// <summary>
        /// The judgement service.
        /// </summary>
        private readonly IFixedIncomeHighVolumeJudgementService judgementService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeRule> logger;

        /// <summary>
        /// The had missing market data.
        /// </summary>
        private bool hadMissingMarketData;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeRule"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="orderFilterService">
        /// The order filter service.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
        /// </param>
        /// <param name="factory">
        /// The factory.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        public FixedIncomeHighVolumeRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseMarketCacheFactory factory,
            IFixedIncomeHighVolumeJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.FutureWindowSize,
                Rules.FixedIncomeHighVolumeIssuance,
                FixedIncomeHighVolumeFactory.Version,
                $"{nameof(FixedIncomeHighVolumeRule)}",
                ruleContext,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this.judgementService = judgementService ?? throw new ArgumentNullException(nameof(judgementService));
            this.dataRequestSubscriber = dataRequestSubscriber ?? throw new ArgumentNullException(nameof(dataRequestSubscriber));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// The clone with factor value for supporting factor value brokering.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighVolumeRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        /// <summary>
        /// The clone object method with shallow clone implementation.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeRule)this.MemberwiseClone();
            clone.BaseClone();

            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} Clone completed for {this.UniverseDateTime}");

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
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunOrderFilledEvent called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunOrderFilledEvent completed for {this.UniverseDateTime}");
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
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} EndOfUniverse completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The filter universe event method used for filtering via CFI code.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilterService.Filter(value);
        }

        /// <summary>
        /// The genesis method on first universe event.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} Genesis called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} Genesis completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The market close any/all.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketClose called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketClose completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The market open any/all.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketOpen called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} MarketOpen completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeRule)} RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        }

        /// <summary>
        /// The run initial submission event delayed for future window.
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
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeRule)} RunRule called at {this.UniverseDateTime}");

            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                this.logger.LogInformation($"RunPostOrderEvent had an empty trade window");

                return;
            }

            var tradedSecurities = tradeWindow.Where(_ => _.OrderFilledVolume.GetValueOrDefault() > 0).ToList();
            var tradedVolume = tradedSecurities.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradedSecurities.ToList());
            var mostRecentTrade = tradeWindow.Peek();

            var dailyBreach = this.CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = this.CheckWindowVolume(mostRecentTrade, tradedVolume);

            if (!dailyBreach && !windowBreach)
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with no daily or window breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForNoBreach(mostRecentTrade);
            }

            if (windowBreach)
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with window breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForWindowBreach(mostRecentTrade, null);
            }

            if (dailyBreach)
            {
                this.logger.LogInformation($"RunPostOrderEvent passing judgement with no daily breach for {mostRecentTrade.Instrument.Identifiers}");
                this.PassJudgementForDailyBreach(mostRecentTrade, null);
            }
        }

        /// <summary>
        /// The run post order event delayed for future window.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The pass judgement for no breach.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        private void PassJudgementForNoBreach(Order mostRecentTrade)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);

            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    null,
                    null);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, false);

            this.judgementService.Judgement(fixedIncomeHighVolumeContext).Wait();
        }

        /// <summary>
        /// The pass judgement for window breach.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="breachDetails">
        /// The breach details.
        /// </param>
        private void PassJudgementForWindowBreach(Order mostRecentTrade, FixedIncomeHighVolumeJudgement.BreachDetails breachDetails)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);

            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    breachDetails,
                    null);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, true);

            this.judgementService.Judgement(fixedIncomeHighVolumeContext).Wait();
        }

        /// <summary>
        /// The pass judgement for daily breach.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="breachDetails">
        /// The breach details.
        /// </param>
        private void PassJudgementForDailyBreach(Order mostRecentTrade, FixedIncomeHighVolumeJudgement.BreachDetails breachDetails)
        {
            var serialisedParameters = JsonConvert.SerializeObject(this.parameters);

            var judgement =
                new FixedIncomeHighVolumeJudgement(
                    this.RuleCtx.RuleParameterId(),
                    this.RuleCtx.CorrelationId(),
                    mostRecentTrade?.ReddeerOrderId?.ToString(),
                    mostRecentTrade?.OrderId?.ToString(),
                    serialisedParameters,
                    this.hadMissingMarketData,
                    false,
                    null,
                    breachDetails);

            var fixedIncomeHighVolumeContext = new FixedIncomeHighVolumeJudgementContext(judgement, true);

            this.judgementService.Judgement(fixedIncomeHighVolumeContext).Wait();
        }


        /// <summary>
        /// The check daily volume.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckDailyVolume(Order order, decimal tradedVolume)
        {
            return false;
        }

        /// <summary>
        /// The check window volume.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="tradedVolume">
        /// The traded volume.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckWindowVolume(Order order, decimal tradedVolume)
        {
            return false;
        }
    }
}