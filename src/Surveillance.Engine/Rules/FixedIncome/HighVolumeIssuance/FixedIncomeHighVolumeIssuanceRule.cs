namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance
{
    using System;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Currency.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
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
    public class FixedIncomeHighVolumeIssuanceRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        /// <summary>
        /// The order filter service for CFI codes.
        /// </summary>
        private readonly IUniverseFixedIncomeOrderFilterService orderFilterService;

        /// <summary>
        /// The rule run parameters.
        /// </summary>
        private readonly IHighVolumeIssuanceRuleFixedIncomeParameters parameters;

        // potentially useful?
        private readonly IMarketTradingHoursService tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;
        private readonly ICurrencyConverterService currencyConverterService;


        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighVolumeIssuanceRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighVolumeIssuanceRule"/> class.
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
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="tradingStackLogger">
        /// The trading stack logger.
        /// </param>
        public FixedIncomeHighVolumeIssuanceRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleContext,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            ILogger<FixedIncomeHighVolumeIssuanceRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.BackwardWindowSize,
                parameters.Windows.FutureWindowSize,
                Rules.FixedIncomeHighVolumeIssuance,
                FixedIncomeHighVolumeFactory.Version,
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)}",
                ruleContext,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.orderFilterService = orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
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
            var clone = (FixedIncomeHighVolumeIssuanceRule)this.Clone();
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
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeIssuanceRule)this.MemberwiseClone();
            clone.BaseClone();

            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone completed for {this.UniverseDateTime}");

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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent completed for {this.UniverseDateTime}");
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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse completed for {this.UniverseDateTime}");
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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis completed for {this.UniverseDateTime}");
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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose completed for {this.UniverseDateTime}");
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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen completed for {this.UniverseDateTime}");
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
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this.logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule completed for {this.UniverseDateTime}");
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
            this.logger.LogInformation($"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunRule called at {this.UniverseDateTime}");

            var tradeWindow = history?.ActiveTradeHistory();

            if (tradeWindow == null
                || !tradeWindow.Any())
            {
                return;
            }

            var tradedSecurities = tradeWindow.Where(_ => _.OrderFilledVolume.GetValueOrDefault() > 0).ToList();
            var tradedVolume = tradedSecurities.Sum(_ => _.OrderFilledVolume.GetValueOrDefault(0));

            var tradePosition = new TradePosition(tradedSecurities.ToList());
            var mostRecentTrade = tradeWindow.Peek();

            var dailyBreach = this.CheckDailyVolume(mostRecentTrade, tradedVolume);
            var windowBreach = this.CheckWindowVolume(mostRecentTrade, tradedVolume);

            // handle judgement marshalling here
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