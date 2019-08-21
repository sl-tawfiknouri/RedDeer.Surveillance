namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class FixedIncomeHighVolumeIssuanceRule : BaseUniverseRule, IFixedIncomeHighVolumeRule
    {
        private readonly IUniverseAlertStream _alertStream;

        private readonly ILogger<FixedIncomeHighVolumeIssuanceRule> _logger;

        private readonly IUniverseFixedIncomeOrderFilterService _orderFilterService;

        private readonly IHighVolumeIssuanceRuleFixedIncomeParameters _parameters;

        public FixedIncomeHighVolumeIssuanceRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseFixedIncomeOrderFilterService orderFilterService,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseMarketCacheFactory factory,
            RuleRunMode runMode,
            IUniverseAlertStream alertStream,
            ILogger<FixedIncomeHighVolumeIssuanceRule> logger,
            ILogger<TradingHistoryStack> tradingStackLogger)
            : base(
                parameters?.Windows?.BackwardWindowSize ?? TimeSpan.FromDays(1),
                parameters?.Windows?.FutureWindowSize ?? TimeSpan.Zero,
                Rules.FixedIncomeHighVolumeIssuance,
                Versioner.Version(1, 0),
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)}",
                ruleCtx,
                factory,
                runMode,
                logger,
                tradingStackLogger)
        {
            this._parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this._orderFilterService =
                orderFilterService ?? throw new ArgumentNullException(nameof(orderFilterService));
            this._alertStream = alertStream ?? throw new ArgumentNullException(nameof(alertStream));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IFactorValue OrganisationFactorValue { get; set; }

        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var clone = (FixedIncomeHighVolumeIssuanceRule)this.Clone();
            clone.OrganisationFactorValue = factor;

            return clone;
        }

        public object Clone()
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone called at {this.UniverseDateTime}");

            var clone = (FixedIncomeHighVolumeIssuanceRule)this.MemberwiseClone();
            clone.BaseClone();

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Clone completed for {this.UniverseDateTime}");

            return clone;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunOrderFilledEvent completed for {this.UniverseDateTime}");
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} EndOfUniverse completed for {this.UniverseDateTime}");
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilterService.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} Genesis completed for {this.UniverseDateTime}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketClose completed for {this.UniverseDateTime}");
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} MarketOpen completed for {this.UniverseDateTime}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunInitialSubmissionRule completed for {this.UniverseDateTime}");
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunRule called at {this.UniverseDateTime}");

            this._logger.LogInformation(
                $"{nameof(FixedIncomeHighVolumeIssuanceRule)} RunRule completed for {this.UniverseDateTime}");
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }
    }
}