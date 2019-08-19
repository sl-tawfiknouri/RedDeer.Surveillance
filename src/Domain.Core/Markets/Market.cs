namespace Domain.Core.Markets
{
    public class Market
    {
        public Market(string id, string marketIdentifierCode, string name, MarketTypes type)
        {
            this.Id = id ?? string.Empty;
            this.MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            this.Name = name ?? string.Empty;
            this.Type = type;
        }

        /// <summary>
        ///     Primary Key
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     MIC
        /// </summary>
        public string MarketIdentifierCode { get; }

        /// <summary>
        ///     Colloquial name for the market
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The market category
        /// </summary>
        public MarketTypes Type { get; }
    }
}