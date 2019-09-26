namespace Domain.Surveillance.Rules
{
    using System.Collections.Generic;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The rule data constraint.
    /// </summary>
    public class RuleDataConstraint : IRuleDataConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDataConstraint"/> class.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="ruleParameterId">
        /// The rule parameter id.
        /// </param>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        public RuleDataConstraint(
            Rules rule,
            string ruleParameterId,
            IReadOnlyCollection<RuleDataSubConstraint> constraints)
        {
            this.Rule = rule;
            this.RuleParameterId = ruleParameterId ?? string.Empty;
            this.Constraints = constraints ?? new RuleDataSubConstraint[0];
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        public Rules Rule { get; }

        /// <summary>
        /// Gets the rule parameter id.
        /// </summary>
        public string RuleParameterId { get; }

        /// <summary>
        /// Gets the constraints.
        /// </summary>
        public IReadOnlyCollection<IRuleDataSubConstraint> Constraints { get; }
    }
}
