namespace Domain.Surveillance.Rules.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Categories;
    using Domain.Surveillance.Scheduling;

    /// <summary>
    /// The RuleDataConstraint interface.
    /// Inheriting Monoid is slightly an ill fit as its for morphisms rather than objects
    /// but it has useful properties for this case
    /// </summary>
    public interface IRuleDataConstraint : IMonoid<IRuleDataConstraint>
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
