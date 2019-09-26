namespace Domain.Surveillance.Rules.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The RuleDataConstraint interface.
    /// </summary>
    public interface IRuleDataConstraint
    {
        /// <summary>
        /// Gets the rule.
        /// </summary>
        Rules Rule { get; }

        /// <summary>
        /// Gets the rule parameter id.
        /// </summary>
        string RuleParameterId { get; }

        /// <summary>
        /// Gets the constraints.
        /// </summary>
        IReadOnlyCollection<IRuleDataSubConstraint> Constraints { get; }
    }
}
