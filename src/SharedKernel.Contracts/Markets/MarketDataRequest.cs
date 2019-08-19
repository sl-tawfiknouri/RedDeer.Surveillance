namespace SharedKernel.Contracts.Markets
{
    using System;

    using Domain.Core.Financial.Assets;

    public class MarketDataRequest
    {
        public MarketDataRequest(
            string id,
            string marketIdentifierCode,
            string cfi,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId,
            bool isCompleted,
            DataSource dataSource)
        {
            this.Id = id;
            this.MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            this.Cfi = cfi ?? string.Empty;
            this.Identifiers = identifiers;
            this.UniverseEventTimeTo = universeEventTimeTo;
            this.UniverseEventTimeFrom = universeEventTimeFrom;
            this.SystemProcessOperationRuleRunId = systemProcessOperationRuleRunId ?? string.Empty;
            this.IsCompleted = isCompleted;
            this.DataSource = dataSource;
        }

        public MarketDataRequest(
            string marketIdentifierCode,
            string cfi,
            InstrumentIdentifiers identifiers,
            DateTime? universeEventTimeFrom,
            DateTime? universeEventTimeTo,
            string systemProcessOperationRuleRunId,
            DataSource dataSource)
            : this(
                null,
                marketIdentifierCode,
                cfi,
                identifiers,
                universeEventTimeFrom,
                universeEventTimeTo,
                systemProcessOperationRuleRunId,
                false,
                dataSource)
        {
        }

        private MarketDataRequest()
        {
        }

        public string Cfi { get; }

        public DataSource DataSource { get; }

        public string Id { get; }

        public InstrumentIdentifiers Identifiers { get; }

        public bool IsCompleted { get; }

        public string MarketIdentifierCode { get; }

        public string SystemProcessOperationRuleRunId { get; }

        public DateTime? UniverseEventTimeFrom { get; }

        public DateTime? UniverseEventTimeTo { get; }

        public static MarketDataRequest Null()
        {
            return new MarketDataRequest();
        }

        public bool IsValid()
        {
            return this.UniverseEventTimeTo >= this.UniverseEventTimeFrom && this.UniverseEventTimeTo != null
                                                                          && this.UniverseEventTimeFrom != null;
        }

        public override string ToString()
        {
            return
                $"{this.MarketIdentifierCode} - {this.UniverseEventTimeFrom} - {this.UniverseEventTimeTo} - {this.Identifiers}";
        }
    }
}