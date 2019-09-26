namespace Domain.Surveillance.Rules
{
    using System;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules.Interfaces;

    using SharedKernel.Contracts.Markets;

    /// <summary>
    /// The rule data sub constraint.
    /// </summary>
    public class RuleDataSubConstraint : IRuleDataSubConstraint
    {
        /// <summary>
        /// The applicable order predicate (strategy pattern).
        /// </summary>
        private readonly Func<Order, bool> applicableOrderPredicateStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleDataSubConstraint"/> class.
        /// </summary>
        /// <param name="forwardOffset">
        /// The forward offset.
        /// </param>
        /// <param name="backwardOffset">
        /// The backward offset.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="applicableOrderPredicateStrategy">
        /// The applicable order predicate strategy.
        /// </param>
        public RuleDataSubConstraint(
            TimeSpan forwardOffset,
            TimeSpan backwardOffset,
            DataSource source,
            Func<Order, bool> applicableOrderPredicateStrategy)
        {
            this.applicableOrderPredicateStrategy = applicableOrderPredicateStrategy;
            this.ForwardOffset = forwardOffset;
            this.Source = source;
            this.BackwardOffset = backwardOffset;
        }

        /// <summary>
        /// Gets the forward offset.
        /// </summary>
        public TimeSpan ForwardOffset { get; }

        /// <summary>
        /// Gets the backward offset.
        /// </summary>
        public TimeSpan BackwardOffset { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public DataSource Source { get; }

        /// <summary>
        /// The predicate.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Predicate(Order order)
        {
            return this.applicableOrderPredicateStrategy.Invoke(order);
        }
    }
}
