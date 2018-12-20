using System;
using DomainV2.Financial;

namespace DomainV2.Markets
{
    public class MarketDataRequest
    {
        public MarketDataRequest(
            string marketIdentifierCode,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId)
        {
            MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            Identifiers = identifiers;
            UniverseEventTimeTo = universeEventTimeTo;
            UniverseEventTimeFrom = universeEventTimeFrom;
            SystemProcessOperationRuleRunId = systemProcessOperationRuleRunId ?? string.Empty;
        }

        public string MarketIdentifierCode { get; }
        public InstrumentIdentifiers Identifiers { get; }
        public DateTime? UniverseEventTimeTo { get; }
        public DateTime? UniverseEventTimeFrom { get; }

        public string SystemProcessOperationRuleRunId { get; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(MarketIdentifierCode)
                && UniverseEventTimeTo >= UniverseEventTimeFrom;
        }

        public override string ToString()
        {
            return $"{MarketIdentifierCode} - {UniverseEventTimeFrom} - {UniverseEventTimeTo} - {Identifiers}";
        }
    }
}