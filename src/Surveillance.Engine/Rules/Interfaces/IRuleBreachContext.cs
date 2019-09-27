namespace Surveillance.Engine.Rules.Rules.Interfaces
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Interfaces;

    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The RuleBreachContext interface.
    /// </summary>
    public interface IRuleBreachContext
    {
        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        string CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the factor value.
        /// </summary>
        IFactorValue FactorValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is back test run.
        /// </summary>
        bool IsBackTestRun { get; set; }

        /// <summary>
        /// Gets or sets the rule parameter id.
        /// rule parameter primary key on client service
        /// </summary>
        string RuleParameterId { get; set; }

        /// <summary>
        /// Gets or sets the rule parameters.
        /// </summary>
        IRuleParameter RuleParameters { get; set; }

        /// <summary>
        /// Gets the security.
        /// </summary>
        FinancialInstrument Security { get; }

        /// <summary>
        /// Gets or sets the system operation id.
        /// </summary>
        string SystemOperationId { get; set; }

        /// <summary>
        /// Gets the trades.
        /// </summary>
        ITradePosition Trades { get; }

        /// <summary>
        /// Gets or sets the universe date time.
        /// </summary>
        DateTime UniverseDateTime { get; set; }

        /// <summary>
        /// Gets the window.
        /// </summary>
        TimeSpan Window { get; }
    }
}