using System;

namespace Domain.V2.Files
{
    /// <summary>
    /// Version 0.2 of the Trade File
    /// </summary>
    public class TradeFileCsv
    {
        /* MARKET IDENTIFICATION */
        public string MarketType { get; set; }
        public string MarketIdentifierCode { get; set; }
        public string MarketName { get; set; }
        /* SECURITY IDENTIFICATION */
        public string InstrumentName { get; set; }
        public string InstrumentCfi { get; set; }
        public string InstrumentIssuerIdentifier { get; set; }
        public string InstrumentClientIdentifier { get; set; }
        public string InstrumentSedol { get; set; }
        public string InstrumentIsin { get; set; }
        public string InstrumentFigi { get; set; }
        public string InstrumentCusip { get; set; }
        public string InstrumentLei { get; set; }
        public string InstrumentExchangeSymbol { get; set; }
        public string InstrumentBloombergTicker { get; set; }
        /* DERIVATIVE FURTHER IDENTIFICATION */
        public string InstrumentUnderlyingName { get; set; }
        public string InstrumentUnderlyingCfi { get; set; }
        public string InstrumentUnderlyingIssuerIdentifier { get; set; }
        public string InstrumentUnderlyingClientIdentifier { get; set; }
        public string InstrumentUnderlyingSedol { get; set; }
        public string InstrumentUnderlyingIsin { get; set; }
        public string InstrumentUnderlyingFigi { get; set; }
        public string InstrumentUnderlyingCusip { get; set; }
        public string InstrumentUnderlyingLei { get; set; }
        public string InstrumentUnderlyingExchangeSymbol { get; set; }
        public string InstrumentUnderlyingBloombergTicker { get; set; }


        /* ORDER LEVEL */
        public string OrderId { get; set; } // the client id for the order
        public DateTime? OrderPlacedDate { get; set; }
        public DateTime? OrderBookedDate { get; set; }
        public DateTime? OrderAmendedDate { get; set; }
        public DateTime? OrderRejectedDate { get; set; }
        public DateTime? OrderCancelledDate { get; set; }
        public DateTime? OrderFilledDate { get; set; }       
        public string OrderType { get; set; }
        public string OrderPosition { get; set; }
        public string OrderCurrency { get; set; }
        public decimal? OrderLimitPrice { get; set; }
        public decimal? OrderAveragePrice { get; set; }
        public long? OrderOrderedVolume { get; set; }
        public long? OrderFilledVolume { get; set; }
        public string OrderPortfolioManager { get; set; }
        public string OrderExecutingBroker { get; set; }
        public string OrderClearingAgent { get; set; }
        public string OrderDealingInstructions { get; set; }
        public string OrderStrategy { get; set; }
        public string OrderRationale { get; set; }
        public string OrderFund { get; set; }
        public string OrderClientAccountAttributionId { get; set; }


        /* TRADE LEVEL */
        public string TradeId { get; set; } // the client id for the trade
        public DateTime? TradePlacedDate { get; set; }
        public DateTime? TradeBookedDate { get; set; }
        public DateTime? TradeAmendedDate { get; set; }
        public DateTime? TradeRejectedDate { get; set; }
        public DateTime? TradeCancelledDate { get; set; }
        public DateTime? TradeFilledDate { get; set; }
        public string TraderId { get; set; }
        public string TradeCounterParty { get; set; }
        public string TradeType { get; set; }
        public string TradePosition { get; set; }
        public string TradeCurrency { get; set; }
        public decimal? TradeLimitPrice { get; set; }
        public decimal? TradeAveragePrice { get; set; }
        public long? TradeOrderedVolume { get; set; }
        public long? TradeFilledVolume { get; set; }
        public decimal? TradeOptionStrikePrice { get; set; }
        public DateTime? TradeOptionExpirationDate { get; set; }
        public string TradeOptionEuropeanAmerican { get; set; }

        
        /* TRANSACTION LEVEL */
        public string TransactionId { get; set; } // the client id for the transaction
        public DateTime? TransactionPlacedDate { get; set; }
        public DateTime? TransactionBookedDate { get; set; }
        public DateTime? TransactionAmendedDate { get; set; }
        public DateTime? TransactionRejectedDate { get; set; }
        public DateTime? TransactionCancelledDate { get; set; }
        public DateTime? TransactionFilledDate { get; set; }
        public string TransactionTraderId { get; set; }
        public string TransactionCounterParty { get; set; }
        public string TransactionType { get; set; }
        public string TransactionPosition { get; set; }
        public string TransactionCurrency { get; set; }
        public decimal? TransactionLimitPrice { get; set; }
        public decimal? TransactionAveragePrice { get; set; }
        public long? TransactionOrderedVolume { get; set; }
        public long? TransactionFilledVolume { get; set; }
    }
}
