using System;
using Domain.Financial.Cfis;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class UniverseEquityOrderFilter : IUniverseEquityOrderFilter
    {
        private readonly ILogger<UniverseEquityOrderFilter> _logger;

        public UniverseEquityOrderFilter(ILogger<UniverseEquityOrderFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns null if it has filtered out the event
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
                _logger.LogError($"{nameof(UniverseEquityOrderFilter)} encountered an unexpected type for the underlying value of a trade event. Not filtering.");
                return universeEvent;
            }

            var cfi = order.Instrument.Cfi;

            if (string.IsNullOrWhiteSpace(cfi))
            {
                _logger.LogError($"{nameof(UniverseEquityOrderFilter)}  tried to process a cfi that was either null or empty for {order.Instrument.Identifiers}. Filtered out unidentifiable instrument.");
                return null;
            }

            var cfiWrap = new Cfi(cfi);
            var filter = cfiWrap.CfiCategory != CfiCategory.Equities;

            if (filter)
            {
                _logger.LogInformation($"{nameof(UniverseEquityOrderFilter)}  filtering out cfi of {cfi} as it did not have a leading character of e");
                return null;
            }

            return universeEvent;
        }
    }
}
