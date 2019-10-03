namespace Domain.Surveillance.Rules
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Categories;
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
            IReadOnlyCollection<IRuleDataSubConstraint> constraints)
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

        /// <summary>
        /// The case.
        /// </summary>
        public IRuleDataConstraint Case => this;

        /// <summary>
        /// The empty.
        /// </summary>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        public static IMonoid<IRuleDataConstraint> Empty()
        {
            return new RuleDataConstraintEmpty();
        }

        /// <summary>
        /// The m empty.
        /// I think this should be returning a derived class from
        /// Rule Data Constraint with an empty type implementation of the monoid
        /// </summary>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        public IMonoid<IRuleDataConstraint> MEmpty()
        {
            return new RuleDataConstraintEmpty();
        }

        /// <summary>
        /// The m append.
        /// </summary>
        /// <param name="_">
        /// The _.
        /// </param>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        public IMonoid<IRuleDataConstraint> MAppend(IMonoid<IRuleDataConstraint> _)
        {
            if (_ == null)
            {
                return this;
            }

            var constraints = this.Constraints.Concat(_.Case.Constraints).ToArray();

            return new RuleDataConstraint(this.Rule, this.RuleParameterId, constraints);
        }

        /// <summary>
        /// The m concatenate.
        /// </summary>
        /// <param name="_">
        /// The _.
        /// </param>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        public IMonoid<IRuleDataConstraint> MConcat(params IMonoid<IRuleDataConstraint>[] _)
        {
            var eval = _?.Where(i => i != null).ToArray();

            if (eval == null || !eval.Any())
            {
                return this;
            }

            return _.Aggregate(this as IMonoid<IRuleDataConstraint>, (a, b) => a.MAppend(b));
        }

        /// <summary>
        /// The rule data constraint empty.
        /// </summary>
        private class RuleDataConstraintEmpty : IRuleDataConstraint
        {
            /// <summary>
            /// Gets the rule.
            /// </summary>
            public Rules Rule { get; } = Rules.HighProfits;

            /// <summary>
            /// Gets the rule parameter id.
            /// </summary>
            public string RuleParameterId { get; } = string.Empty;

            /// <summary>
            /// Gets the constraints.
            /// </summary>
            public IReadOnlyCollection<IRuleDataSubConstraint> Constraints { get; } = new IRuleDataSubConstraint[0];

            /// <summary>
            /// The case.
            /// </summary>
            public IRuleDataConstraint Case => this;

            /// <summary>
            /// The m empty.
            /// </summary>
            /// <returns>
            /// The <see cref="IMonoid"/>.
            /// </returns>
            public IMonoid<IRuleDataConstraint> MEmpty()
            {
                return this;
            }

            /// <summary>
            /// The m append.
            /// </summary>
            /// <param name="_">
            /// The _.
            /// </param>
            /// <returns>
            /// The <see cref="IMonoid"/>.
            /// </returns>
            public IMonoid<IRuleDataConstraint> MAppend(IMonoid<IRuleDataConstraint> _)
            {
                return _;
            }

            /// <summary>
            /// The m concat.
            /// </summary>
            /// <param name="_">
            /// The _.
            /// </param>
            /// <returns>
            /// The <see cref="IMonoid"/>.
            /// </returns>
            public IMonoid<IRuleDataConstraint> MConcat(params IMonoid<IRuleDataConstraint>[] _)
            {
                if (_ == null || !_.Any())
                {
                    return this;
                }

                return _.First().MConcat(_.Skip(1).ToArray());
            }
        }
    }
}
