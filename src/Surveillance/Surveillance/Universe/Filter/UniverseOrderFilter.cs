using System;
using System.Linq;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Filter
{
    public class UniverseOrderFilter : IUniverseOrderFilter
    {
        private readonly ILogger<UniverseOrderFilter> _logger;

        public UniverseOrderFilter(ILogger<UniverseOrderFilter> logger)
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
                case UniverseStateEvent.TradeReddeer:
                case UniverseStateEvent.TradeReddeerSubmitted:
                    break;
                default:
                    return universeEvent;
            }

            var order = universeEvent.UnderlyingEvent as Order;

            if (order == null)
            {
                _logger.LogError($"UniverseOrderFilter encountered an unexpected type for the underlying value of a trade event. Not filtering.");
                return universeEvent;
            }

            var cfi = order.Instrument.Cfi;

            if (string.IsNullOrWhiteSpace(cfi))
            {
                _logger.LogError($"UniverseOrderFilter tried to process a cfi that was either null or empty for {order.Instrument.Identifiers}. Filtered out unidentifiable instrument.");
                return null;
            }

            var leadingCharacter = cfi.ToLower().First();

            return leadingCharacter == 'e'
                ? universeEvent
                : null;
        }
    }
}
