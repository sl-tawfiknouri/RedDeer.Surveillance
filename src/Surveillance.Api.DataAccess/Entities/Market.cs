using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Market : IMarket
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// MIC
        /// </summary>
        public string MarketId { get; set; }

        /// <summary>
        /// Colloquial name for the market
        /// </summary>
        public string MarketName { get; set; }
    }
}
