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
    /// The universe fixed income order filter service.
    /// </summary>
    public class UniverseFixedIncomeOrderFilterService : IUniverseFixedIncomeOrderFilterService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniverseFixedIncomeOrderFilterService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseFixedIncomeOrderFilterService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniverseFixedIncomeOrderFilterService(ILogger<UniverseFixedIncomeOrderFilterService> logger)
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

            var excludeOrder = this.Filter(order);

            if (excludeOrder)
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
                return true;
            }

            var cfi = order.Instrument.Cfi;

            if (string.IsNullOrWhiteSpace(cfi))
            {
                this.logger.LogError(
                    $"tried to process a cfi that was either null or empty for {order.Instrument.Identifiers}. Filtered out unidentifiable instrument.");
                return true;
            }

            var cfiWrap = new Cfi(cfi);
            var filter = cfiWrap.CfiCategory != CfiCategory.DebtInstrument;

            if (filter)
            {
                this.logger.LogInformation($"filtering out cfi of {cfi} as it did not have a leading character of d");
                return true;
            }

            return false;
        }
    }
}