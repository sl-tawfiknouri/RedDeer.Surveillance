namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Domain.Core.Financial.Cfis;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The universe equity order filter service.
    /// </summary>
    public class UniverseEquityOrderFilterService : IUniverseEquityOrderFilterService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniverseEquityOrderFilterService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseEquityOrderFilterService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniverseEquityOrderFilterService(ILogger<UniverseEquityOrderFilterService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns null if it has filtered out the event
        /// </summary>
        /// <param name="universeEvent">
        /// The universe Event.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        public IUniverseEvent Filter(IUniverseEvent universeEvent)
        {
            switch (universeEvent.StateChange)
            {
                case UniverseStateEvent.Order:
                case UniverseStateEvent.OrderPlaced:
                    break;
                default:
                    return universeEvent;
            }

            var order = universeEvent.UnderlyingEvent as Order;

            if (order == null)
            {
                this.logger.LogError(
                    "encountered an unexpected type for the underlying value of a trade event. Not filtering.");
                return universeEvent;
            }

            var excludeUniverseEvent = this.Filter(order);

            if (excludeUniverseEvent)
            {
                return null;
            }
            
            return universeEvent;
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Filter(Order order)
        {
            if (order == null)
            {
                this.logger.LogError(
                    "encountered an unexpected type for the underlying value of a trade event. Not filtering.");

                return false;
            }

            var cfi = order.Instrument.Cfi;

            if (string.IsNullOrWhiteSpace(cfi))
            {
                this.logger.LogError(
                    $"tried to process a cfi that was either null or empty for {order.Instrument.Identifiers}. Filtered out unidentifiable instrument.");

                return true;
            }

            var cfiWrap = new Cfi(cfi);
            var filter = cfiWrap.CfiCategory != CfiCategory.Equities;

            if (filter)
            {
                this.logger.LogInformation($"filtering out cfi of {cfi} as it did not have a leading character of e");

                return true;
            }

            return false;
        }
    }
}