namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The UniverseRuleSubscriptionSummary interface.
    /// </summary>
    public interface IUniverseRuleSubscriptionSummary
    {
        /// <summary>
        /// Gets or sets the rule ids.
        /// </summary>
        IReadOnlyCollection<string> RuleIds { get; }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        IReadOnlyCollection<IUniverseRule> Rules { get; }
    }
}
