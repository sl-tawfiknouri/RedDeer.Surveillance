using System;

namespace DomainV2.Files
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
        public string OrderPlacedDate { get; set; }
        public string OrderBookedDate { get; set; }
        public string OrderAmendedDate { get; set; }
        public string OrderRejectedDate { get; set; }
        public string OrderCancelledDate { get; set; }
        public string OrderFilledDate { get; set; }       
        public string OrderType { get; set; }
        public string OrderPosition { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderLimitPrice { get; set; }
        public string OrderAveragePrice { get; set; }
        public string OrderOrderedVolume { get; set; }
        public string OrderFilledVolume { get; set; }
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
        public string TradePlacedDate { get; set; }
        public string TradeBookedDate { get; set; }
        public string TradeAmendedDate { get; set; }
        public string TradeRejectedDate { get; set; }
        public string TradeCancelledDate { get; set; }
        public string TradeFilledDate { get; set; }
        public string TraderId { get; set; }
        public string TradeCounterParty { get; set; }
        public string TradeType { get; set; }
        public string TradePosition { get; set; }
        public string TradeCurrency { get; set; }
        public string TradeLimitPrice { get; set; }
        public string TradeAveragePrice { get; set; }
        public string TradeOrderedVolume { get; set; }
        public string TradeFilledVolume { get; set; }
        public string TradeOptionStrikePrice { get; set; }
        public string TradeOptionExpirationDate { get; set; }
        public string TradeOptionEuropeanAmerican { get; set; }

        
        /* TRANSACTION LEVEL */
        public string TransactionId { get; set; } // the client id for the transaction
        public string TransactionPlacedDate { get; set; }
        public string TransactionBookedDate { get; set; }
        public string TransactionAmendedDate { get; set; }
        public string TransactionRejectedDate { get; set; }
        public string TransactionCancelledDate { get; set; }
        public string TransactionFilledDate { get; set; }
        public string TransactionTraderId { get; set; }
        public string TransactionCounterParty { get; set; }
        public string TransactionType { get; set; }
        public string TransactionPosition { get; set; }
        public string TransactionCurrency { get; set; }
        public string TransactionLimitPrice { get; set; }
        public string TransactionAveragePrice { get; set; }
        public string TransactionOrderedVolume { get; set; }
        public string TransactionFilledVolume { get; set; }


        public int RowId { get; set; }
    }
}
