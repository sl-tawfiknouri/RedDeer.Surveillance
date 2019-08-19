namespace Surveillance.Api.DataAccess.Entities
{
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class Market : IMarket
    {
        /// <summary>
        ///     Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     MIC
        /// </summary>
        public string MarketId { get; set; }

        /// <summary>
        ///     Colloquial name for the market
        /// </summary>
        public string MarketName { get; set; }
    }
}