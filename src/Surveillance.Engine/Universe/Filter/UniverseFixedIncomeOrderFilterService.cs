namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Domain.Core.Financial.Cfis;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class UniverseFixedIncomeOrderFilterService : IUniverseFixedIncomeOrderFilterService
    {
        private readonly ILogger<UniverseFixedIncomeOrderFilterService> _logger;

        public UniverseFixedIncomeOrderFilterService(ILogger<UniverseFixedIncomeOrderFilterService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Returns null if it has filtered out the event
        /// </summary>
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
                this._logger.LogError(
                    "encountered an unexpected type for the underlying value of a trade event. Not filtering.");
                return universeEvent;
            }

            var cfi = order.Instrument.Cfi;

            if (string.IsNullOrWhiteSpace(cfi))
            {
                this._logger.LogError(
                    $"tried to process a cfi that was either null or empty for {order.Instrument.Identifiers}. Filtered out unidentifiable instrument.");
                return null;
            }

            var cfiWrap = new Cfi(cfi);
            var filter = cfiWrap.CfiCategory != CfiCategory.DebtInstrument;

            if (filter)
            {
                this._logger.LogInformation($"filtering out cfi of {cfi} as it did not have a leading character of d");
                return null;
            }

            return universeEvent;
        }
    }
}