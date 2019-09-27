namespace Surveillance.Engine.Rules.Universe.Filter.Interfaces
{
    using Domain.Core.Trading.Orders;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The UniverseOrderFilter interface.
    /// </summary>
    public interface IUniverseOrderFilter
    {
        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="universeEvent">
        /// The universe event.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        IUniverseEvent Filter(IUniverseEvent universeEvent);

        /// <summary>
        /// The filter.
        /// Underlying implementation for the IUniverseEvent wrapper
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Filter(Order order);
    }
}