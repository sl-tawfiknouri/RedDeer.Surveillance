namespace Surveillance.Engine.Rules.Universe
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    /// <summary>
    /// The universe rule subscription summary.
    /// </summary>
    public class UniverseRuleSubscriptionSummary : IUniverseRuleSubscriptionSummary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseRuleSubscriptionSummary"/> class.
        /// </summary>
        /// <param name="ruleIds">
        /// The rule ids.
        /// </param>
        /// <param name="rules">
        /// The rules.
        /// </param>
        public UniverseRuleSubscriptionSummary(
            IReadOnlyCollection<string> ruleIds,
            IReadOnlyCollection<IUniverseRule> rules)
        {
            this.RuleIds = ruleIds ?? new string[0];
            this.Rules = rules ?? new IUniverseRule[0];
        }

        /// <summary>
        /// Gets the rule ids.
        /// </summary>
        public IReadOnlyCollection<string> RuleIds { get; }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        public IReadOnlyCollection<IUniverseRule> Rules { get; }
    }
}
