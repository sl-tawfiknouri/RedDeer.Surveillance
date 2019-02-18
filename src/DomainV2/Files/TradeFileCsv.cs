namespace Domain.Files
{
    /// <summary>
    /// Version 0.3 of the Trade File
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

        // versioning
        public string OrderVersion { get; set; }
        public string OrderVersionLinkId { get; set; }
        public string OrderGroupId { get; set; }
        
        // dates
        public string OrderPlacedDate { get; set; }
        public string OrderBookedDate { get; set; }
        public string OrderAmendedDate { get; set; }
        public string OrderRejectedDate { get; set; }
        public string OrderCancelledDate { get; set; }
        public string OrderFilledDate { get; set; }

        // order
        public string OrderType { get; set; }
        public string OrderDirection { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderSettlementCurrency { get; set; }
        public string OrderCleanDirty { get; set; }
        public string OrderAccumulatedInterest { get; set; }
        public string OrderLimitPrice { get; set; }
        public string OrderAverageFillPrice { get; set; }
        public string OrderOrderedVolume { get; set; }
        public string OrderFilledVolume { get; set; }
        public string OrderTraderId { get; set; }
        public string OrderTraderName { get; set; }
        public string OrderClearingAgent { get; set; }
        public string OrderDealingInstructions { get; set; }

        // options
        public string OrderOptionStrikePrice { get; set; }
        public string OrderOptionExpirationDate { get; set; }
        public string OrderOptionEuropeanAmerican { get; set; }



        /* TRADE LEVEL */
        public string DealerOrderId { get; set; } // the client id for the trade

        // versioning
        public string DealerOrderVersion { get; set; }
        public string DealerOrderVersionLinkId { get; set; }
        public string DealerOrderGroupId { get; set; }

        // dates
        public string DealerOrderPlacedDate { get; set; }
        public string DealerOrderBookedDate { get; set; }
        public string DealerOrderAmendedDate { get; set; }
        public string DealerOrderRejectedDate { get; set; }
        public string DealerOrderCancelledDate { get; set; }
        public string DealerOrderFilledDate { get; set; }

        // dealer order
        public string DealerOrderDealerId { get; set; }
        public string DealerOrderDealerName { get; set; }
        public string DealerOrderNotes { get; set; }
        public string DealerOrderCounterParty { get; set; }
        public string DealerOrderType { get; set; }
        public string DealerOrderDirection { get; set; }
        public string DealerOrderCurrency { get; set; }
        public string DealerOrderSettlementCurrency { get; set; }

        public string DealerOrderCleanDirty { get; set; }
        public string DealerOrderAccumulatedInterest { get; set; }


        public string DealerOrderLimitPrice { get; set; }
        public string DealerOrderAverageFillPrice { get; set; }
        public string DealerOrderOrderedVolume { get; set; }
        public string DealerOrderFilledVolume { get; set; }

        // options
        public string DealerOrderOptionStrikePrice { get; set; }
        public string DealerOrderOptionExpirationDate { get; set; }
        public string DealerOrderOptionEuropeanAmerican { get; set; }

        /* IO */
        public int RowId { get; set; }
    }
}
