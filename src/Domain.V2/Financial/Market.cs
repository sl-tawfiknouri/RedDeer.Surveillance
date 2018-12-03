namespace Domain.V2.Financial
{
    public class Market
    {
        public Market(
            string marketIdentifierCode,
            string name,
            MarketTypes type)
        {
            MarketIdentifierCode = marketIdentifierCode;
            Name = name;
            Type = type;
        }

        /// <summary>
        /// MIC
        /// </summary>
        public string MarketIdentifierCode { get; }

        /// <summary>
        /// Colloquial name for the market
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The market category
        /// </summary>
        public MarketTypes Type { get; }
    }
}
