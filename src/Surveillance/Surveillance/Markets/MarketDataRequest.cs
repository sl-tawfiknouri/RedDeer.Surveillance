using System;
using DomainV2.Financial;

namespace Surveillance.Markets
{
    public class MarketDataRequest
    {
        public MarketDataRequest(
            string marketIdentifierCode,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeTo,
            DateTime? universeEventTimeFrom)
        {
            MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            Identifiers = identifiers;
            UniverseEventTimeTo = universeEventTimeTo;
            UniverseEventTimeFrom = universeEventTimeFrom;
        }

        public string MarketIdentifierCode { get; }
        public InstrumentIdentifiers Identifiers { get; }
        public DateTime? UniverseEventTimeTo { get; }
        public DateTime? UniverseEventTimeFrom { get; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(MarketIdentifierCode);
        }

        public override string ToString()
        {
            return $"{MarketIdentifierCode} - {UniverseEventTimeFrom} - {UniverseEventTimeTo} - {Identifiers}";
        }
    }
}