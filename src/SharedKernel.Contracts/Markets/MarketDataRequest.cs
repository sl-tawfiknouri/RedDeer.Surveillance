using System;

namespace Domain.Markets
{
    public class MarketDataRequest
    {
        private MarketDataRequest()
        { }
        
        public MarketDataRequest(
            string id,
            string marketIdentifierCode,
            string cfi,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId,
            bool isCompleted)
        {
            Id = id;
            MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            Cfi = cfi ?? string.Empty;
            Identifiers = identifiers;
            UniverseEventTimeTo = universeEventTimeTo;
            UniverseEventTimeFrom = universeEventTimeFrom;
            SystemProcessOperationRuleRunId = systemProcessOperationRuleRunId ?? string.Empty;
            IsCompleted = isCompleted;
        }

        public MarketDataRequest(
            string marketIdentifierCode,
            string cfi,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId):
            this(
                null, 
                marketIdentifierCode,
                cfi,
                identifiers,
                universeEventTimeFrom,
                universeEventTimeTo,
                systemProcessOperationRuleRunId,
                false)
        { }

        public string Id { get; }
        public string MarketIdentifierCode { get; }
        public string Cfi { get; }
        public InstrumentIdentifiers Identifiers { get; }
        public DateTime? UniverseEventTimeTo { get; }
        public DateTime? UniverseEventTimeFrom { get; }

        public string SystemProcessOperationRuleRunId { get; }

        public bool IsCompleted { get; }

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

        public static MarketDataRequest Null()
        {
            return new MarketDataRequest();
        }
    }
}