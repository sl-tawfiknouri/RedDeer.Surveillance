using System;
using DomainV2.Financial;

namespace DomainV2.Markets
{
    public class MarketDataRequest
    {
        public MarketDataRequest(
            string marketIdentifierCode,
            string cfi,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId)
        {
            MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            Cfi = cfi ?? string.Empty;
            Identifiers = identifiers;
            UniverseEventTimeTo = universeEventTimeTo;
            UniverseEventTimeFrom = universeEventTimeFrom;
            SystemProcessOperationRuleRunId = systemProcessOperationRuleRunId ?? string.Empty;
        }

        public string MarketIdentifierCode { get; }
        public string Cfi { get; }
        public InstrumentIdentifiers Identifiers { get; }
        public DateTime? UniverseEventTimeTo { get; }
        public DateTime? UniverseEventTimeFrom { get; }

        public string SystemProcessOperationRuleRunId { get; }

        public bool IsValid()
        {
            return UniverseEventTimeTo >= UniverseEventTimeFrom
                   && UniverseEventTimeTo != null
                   && UniverseEventTimeFrom != null;
        }

        public override string ToString()
        {
            return $"{MarketIdentifierCode} - {UniverseEventTimeFrom} - {UniverseEventTimeTo} - {Identifiers}";
        }
    }
}