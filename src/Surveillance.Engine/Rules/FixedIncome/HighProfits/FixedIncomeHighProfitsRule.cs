namespace Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    /// The fixed income high profits rule.
    /// </summary>
    public class FixedIncomeHighProfitsRule : IFixedIncomeHighProfitsRule
    {
        /// <summary>
        /// The fixed income parameters.
        /// </summary>
        private readonly IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters;

        /// <summary>
        /// The fixed income market closure rule.
        /// </summary>
        private readonly IFixedIncomeHighProfitsMarketClosureRule marketClosureRule;

        /// <summary>
        /// The fixed income high profit stream rule.
        /// </summary>
        private readonly IFixedIncomeHighProfitsStreamRule streamRule;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FixedIncomeHighProfitsRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedIncomeHighProfitsRule"/> class.
        /// </summary>
        /// <param name="fixedIncomeParameters">
        /// The fixed income parameters.
        /// </param>
        /// <param name="streamRule">
        /// The stream rule.
        /// </param>
        /// <param name="marketClosureRule">
        /// The market closure rule.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FixedIncomeHighProfitsRule(
            IHighProfitsRuleFixedIncomeParameters fixedIncomeParameters,
            IFixedIncomeHighProfitsStreamRule streamRule,
            IFixedIncomeHighProfitsMarketClosureRule marketClosureRule,
            ILogger<FixedIncomeHighProfitsRule> logger)
        {
            this.fixedIncomeParameters =
                fixedIncomeParameters ?? throw new ArgumentNullException(nameof(fixedIncomeParameters));
            this.streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            this.marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// Gets the rule definition.
        /// </summary>
        public Rules Rule { get; } = Rules.FixedIncomeHighProfits;

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; } = FixedIncomeHighProfitFactory.Version;

        /// <summary>
        /// The cloning support for factor values.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new FixedIncomeHighProfitsRule(
                this.fixedIncomeParameters,
                this.streamRule.Clone(factor) as IFixedIncomeHighProfitsStreamRule, 
                this.marketClosureRule.Clone(factor) as IFixedIncomeHighProfitsMarketClosureRule,
                this.logger);

            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        /// <summary>
        /// The clone with object typing returns shallow clones but with deep cloned child rules for
        /// stream and market closure.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var cloneRule = new FixedIncomeHighProfitsRule(
                this.fixedIncomeParameters,
                (IFixedIncomeHighProfitsStreamRule)this.streamRule.Clone(),
                (IFixedIncomeHighProfitsMarketClosureRule)this.marketClosureRule.Clone(),
                this.logger);

            return cloneRule;
        }

        /// <summary>
        /// The on completed event trigger.
        /// </summary>
        public void OnCompleted()
        {
            this.logger.LogInformation(
                "OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            this.streamRule.OnCompleted();
            this.marketClosureRule.OnCompleted();
        }

        /// <summary>
        /// The on error event trigger
        /// </summary>
        /// <param name="error">
        /// exception that occurred
        /// </param>
        public void OnError(Exception error)
        {
            this.logger.LogError("OnError() event received", error);
            this.streamRule.OnError(error);
            this.marketClosureRule.OnError(error);
        }

        /// <summary>
        /// The on next event trigger
        /// </summary>
        /// <param name="value">
        /// universe event to progress
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            this.logger.LogInformation(
                $"OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            if (this.fixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                this.streamRule.OnNext(value);
                this.marketClosureRule.OnNext(value);
            }

            if (this.fixedIncomeParameters.PerformHighProfitDailyAnalysis
                && !this.fixedIncomeParameters.PerformHighProfitWindowAnalysis)
            {
                this.marketClosureRule.OnNext(value);
            }
        } 
    }
}