namespace Domain.Trades.Orders
{
    /// <summary>
    /// CSV Representation of a trade order frame for mapping from csv and onto trade order frame
    /// </summary>
    public class TradeOrderFrameCsv
    {
        public string OrderType { get; set; }

        public string MarketId { get; set; }
        public string MarketName { get; set; }

        public string SecurityName { get; set; }
        public string SecurityCfi { get; set; }

        public string SecurityClientIdentifier { get; set; }
        public string SecuritySedol { get; set; }
        public string SecurityIsin { get; set; }
        public string SecurityFigi { get; set; }
        public string SecurityCusip { get; set; }
        public string SecurityExchangeSymbol { get; set; }

        public string LimitPrice { get; set; }

        public string TradeSubmittedOn { get; set; }
        public string StatusChangedOn { get; set; }

        public string Volume { get; set; }
        public string OrderPosition { get; set; }


        public string TraderId { get; set; }
        public string TraderClientAttributionId { get; set; }

        public string PartyBrokerId { get; set; }
        public string CounterPartyBrokerId { get; set; }


        public string OrderStatus { get; set; }
        public string Currency { get; set; }
    }
}