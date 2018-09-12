namespace Domain.Trades.Orders
{
    /// <summary>
    /// CSV Representation of a trade order frame for mapping from csv and onto trade order frame
    /// </summary>
    public class TradeOrderFrameCsv
    {
        public string StatusChangedOn { get; set; }
        public string MarketId { get; set; }
        public string MarketAbbreviation { get; set; }
        public string MarketName { get; set; }
        public string SecurityClientIdentifier { get; set; }
        public string SecuritySedol { get; set; }
        public string SecurityIsin { get; set; }
        public string SecurityFigi { get; set; }
        public string SecurityName { get; set; }
        public string OrderType { get; set; }
        public string OrderDirection { get; set; }
        public string OrderStatus { get; set; }
        public string Volume { get; set; }
        public string LimitPrice { get; set; }
        public string Currency { get; set; }
    }
}