﻿namespace Domain.Core.Markets
{
    public class Market
    {
        public Market(
            string id,
            string marketIdentifierCode,
            string name,
            MarketTypes type)
        {
            Id = id ?? string.Empty;
            MarketIdentifierCode = marketIdentifierCode ?? string.Empty;
            Name = name ?? string.Empty;
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

        /// <summary>
        /// Primary Key
        /// </summary>
        public string Id { get; }
    }
}
