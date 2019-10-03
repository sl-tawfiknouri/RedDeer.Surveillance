namespace Domain.Surveillance.Rules.Interfaces
{
    using System;

    using Domain.Core.Trading.Orders;

    using SharedKernel.Contracts.Markets;

    /// <summary>
    /// The RuleDataSubConstraint interface.
    /// </summary>
    public interface IRuleDataSubConstraint
    {
        /// <summary>
        /// Gets the forward offset.
        /// </summary>
        TimeSpan ForwardOffset { get; }

        /// <summary>
        /// Gets the backward offset.
        /// </summary>
        TimeSpan BackwardOffset { get; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        DataSource Source { get; }

        /// <summary>
        /// The predicate.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Predicate(Order order);
    }
}
