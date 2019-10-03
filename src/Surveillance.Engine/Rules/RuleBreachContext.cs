namespace Surveillance.Engine.Rules.Rules
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The rule breach context.
    /// </summary>
    public class RuleBreachContext : IRuleBreachContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBreachContext"/> class.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <param name="trades">
        /// The trades.
        /// </param>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="isBackTestRun">
        /// The is back test run.
        /// </param>
        /// <param name="ruleParameterId">
        /// The rule parameter id.
        /// </param>
        /// <param name="systemOperationId">
        /// The system operation id.
        /// </param>
        /// <param name="correlationId">
        /// The correlation id.
        /// </param>
        /// <param name="factorValue">
        /// The factor value.
        /// </param>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        /// <param name="universeDateTime">
        /// The universe date time.
        /// </param>
        public RuleBreachContext(
            TimeSpan window,
            ITradePosition trades,
            FinancialInstrument security,
            bool isBackTestRun,
            string ruleParameterId,
            string systemOperationId,
            string correlationId,
            IFactorValue factorValue,
            IRuleParameter ruleParameters,
            DateTime universeDateTime)
        {
            this.Window = window;
            this.Trades = trades;
            this.Security = security;
            this.IsBackTestRun = isBackTestRun;
            this.RuleParameterId = ruleParameterId;
            this.SystemOperationId = systemOperationId;
            this.CorrelationId = correlationId;
            this.FactorValue = factorValue;
            this.RuleParameters = ruleParameters;
            this.UniverseDateTime = universeDateTime;
        }

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the factor value.
        /// </summary>
        public IFactorValue FactorValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is back test run.
        /// </summary>
        public bool IsBackTestRun { get; set; }

        /// <summary>
        /// Gets or sets the rule parameter id.
        /// </summary>
        public string RuleParameterId { get; set; }

        /// <summary>
        /// Gets or sets the rule parameters.
        /// </summary>
        public IRuleParameter RuleParameters { get; set; }

        /// <summary>
        /// Gets the security.
        /// </summary>
        public FinancialInstrument Security { get; }

        /// <summary>
        /// Gets or sets the system operation id.
        /// </summary>
        public string SystemOperationId { get; set; }

        /// <summary>
        /// Gets the trades.
        /// </summary>
        public ITradePosition Trades { get; }

        /// <summary>
        /// Gets or sets the universe date time.
        /// </summary>
        public DateTime UniverseDateTime { get; set; }

        /// <summary>
        /// Gets the window.
        /// </summary>
        public TimeSpan Window { get; }
    }
}