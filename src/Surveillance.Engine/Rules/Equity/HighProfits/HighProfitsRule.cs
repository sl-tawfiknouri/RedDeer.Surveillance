namespace Surveillance.Engine.Rules.Rules.Equity.HighProfits
{
    using System;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    ///     Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        /// <summary>
        /// The equities parameters.
        /// </summary>
        private readonly IHighProfitsRuleEquitiesParameters equitiesParameters;

        /// <summary>
        /// The market closure rule.
        /// </summary>
        private readonly IHighProfitMarketClosureRule marketClosureRule;

        /// <summary>
        /// The stream rule.
        /// </summary>
        private readonly IHighProfitStreamRule streamRule;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HighProfitsRule> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighProfitsRule"/> class.
        /// </summary>
        /// <param name="equitiesParameters">
        /// The equities parameters.
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
        public HighProfitsRule(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            IHighProfitStreamRule streamRule,
            IHighProfitMarketClosureRule marketClosureRule,
            ILogger<HighProfitsRule> logger)
        {
            this.equitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));
            this.streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            this.marketClosureRule = marketClosureRule ?? throw new ArgumentNullException(nameof(marketClosureRule));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the organization factor value.
        /// </summary>
        public IFactorValue OrganisationFactorValue { get; set; } = FactorValue.None;

        /// <summary>
        /// Gets the rule.
        /// </summary>
        public Rules Rule { get; } = Rules.HighProfits;

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; } = EquityRuleHighProfitFactory.Version;

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public IRuleDataConstraint DataConstraints()
        {
            return this.marketClosureRule.DataConstraints().MConcat(this.streamRule.DataConstraints())?.Case;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseCloneableRule"/>.
        /// </returns>
        public IUniverseCloneableRule Clone(IFactorValue factor)
        {
            var cloneRule = new HighProfitsRule(
                this.equitiesParameters,
                (IHighProfitStreamRule)this.streamRule.Clone(factor),
                (IHighProfitMarketClosureRule)this.marketClosureRule.Clone(factor),
                this.logger);

            cloneRule.OrganisationFactorValue = factor;

            return cloneRule;
        }

        /// <summary>
        /// The clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Clone()
        {
            var cloneRule = new HighProfitsRule(
                this.equitiesParameters,
                (IHighProfitStreamRule)this.streamRule.Clone(),
                (IHighProfitMarketClosureRule)this.marketClosureRule.Clone(),
                this.logger);

            return cloneRule;
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.logger.LogInformation(
                "OnCompleted() event received. Passing onto high profit and high profit market close rules.");
            this.streamRule.OnCompleted();
            this.marketClosureRule.OnCompleted();
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.logger.LogError(error, "OnCompleted() event received");
            this.streamRule.OnError(error);
            this.marketClosureRule.OnError(error);
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            this.logger.LogInformation(
                $"OnNext() event received at {value.EventTime}. Passing onto high profit and high profit market close rules.");

            // if removing the market closure rule
            // ensure that the alert subscriber is also updated to remove expectation of 2x flush events
            if (this.equitiesParameters.PerformHighProfitWindowAnalysis)
            {
                this.streamRule.OnNext(value);
                this.marketClosureRule.OnNext(value);
            }

            if (this.equitiesParameters.PerformHighProfitDailyAnalysis
                && !this.equitiesParameters.PerformHighProfitWindowAnalysis)
            {
                this.marketClosureRule.OnNext(value);
            }
        }
    }
}