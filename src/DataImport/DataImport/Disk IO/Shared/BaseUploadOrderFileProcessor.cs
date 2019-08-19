namespace DataImport.Disk_IO.Shared
{
    using CsvHelper;

    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Orders;

    public abstract class BaseUploadOrderFileProcessor : BaseUploadFileProcessor<OrderFileContract, Order>
    {
        protected BaseUploadOrderFileProcessor(ILogger logger)
            : base(logger, "Upload Trade File Processor")
        {
        }

        protected override OrderFileContract MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                this.Logger.LogInformation(
                    $"Upload Trade File Processor received a null record to map with row id {rowId}");
                return null;
            }

            this.Logger.LogInformation("Upload Trade File Processor about to map raw record to csv dto");
            var tradeCsv = new OrderFileContract
                               {
                                   MarketType = this.PreProcess(rawRecord["MarketType"]),
                                   MarketIdentifierCode = this.PreProcess(rawRecord["MarketIdentifierCode"]),
                                   MarketName = this.PreProcess(rawRecord["MarketName"]),
                                   InstrumentName = this.PreProcess(rawRecord["InstrumentName"]),
                                   InstrumentCfi = this.PreProcess(rawRecord["InstrumentCfi"]),
                                   InstrumentIssuerIdentifier =
                                       this.PreProcess(rawRecord["InstrumentIssuerIdentifier"]),
                                   InstrumentClientIdentifier =
                                       this.PreProcess(rawRecord["InstrumentClientIdentifier"]),
                                   InstrumentSedol = this.PreProcess(rawRecord["InstrumentSedol"]),
                                   InstrumentIsin = this.PreProcess(rawRecord["InstrumentIsin"]),
                                   InstrumentFigi = this.PreProcess(rawRecord["InstrumentFigi"]),
                                   InstrumentCusip = this.PreProcess(rawRecord["InstrumentCusip"]),
                                   InstrumentLei = this.PreProcess(rawRecord["InstrumentLei"]),
                                   InstrumentExchangeSymbol = this.PreProcess(rawRecord["InstrumentExchangeSymbol"]),
                                   InstrumentBloombergTicker = this.PreProcess(rawRecord["InstrumentBloombergTicker"]),
                                   InstrumentUnderlyingName = this.PreProcess(rawRecord["InstrumentUnderlyingName"]),
                                   InstrumentUnderlyingCfi = this.PreProcess(rawRecord["InstrumentUnderlyingCfi"]),
                                   InstrumentUnderlyingIssuerIdentifier =
                                       this.PreProcess(rawRecord["InstrumentUnderlyingIssuerIdentifier"]),
                                   InstrumentUnderlyingClientIdentifier =
                                       this.PreProcess(rawRecord["InstrumentUnderlyingClientIdentifier"]),
                                   InstrumentUnderlyingSedol = this.PreProcess(rawRecord["InstrumentUnderlyingSedol"]),
                                   InstrumentUnderlyingIsin = this.PreProcess(rawRecord["InstrumentUnderlyingIsin"]),
                                   InstrumentUnderlyingFigi = this.PreProcess(rawRecord["InstrumentUnderlyingFigi"]),
                                   InstrumentUnderlyingCusip = this.PreProcess(rawRecord["InstrumentUnderlyingCusip"]),
                                   InstrumentUnderlyingLei = this.PreProcess(rawRecord["InstrumentUnderlyingLei"]),
                                   InstrumentUnderlyingExchangeSymbol =
                                       this.PreProcess(rawRecord["InstrumentUnderlyingExchangeSymbol"]),
                                   InstrumentUnderlyingBloombergTicker =
                                       this.PreProcess(rawRecord["InstrumentUnderlyingBloombergTicker"]),
                                   OrderId = this.PreProcess(rawRecord["OrderId"]),
                                   OrderVersion = this.PreProcess(rawRecord["OrderVersion"]),
                                   OrderVersionLinkId = this.PreProcess(rawRecord["OrderVersionLinkId"]),
                                   OrderGroupId = this.PreProcess(rawRecord["OrderGroupId"]),
                                   OrderPlacedDate = this.PreProcess(rawRecord["OrderPlacedDate"]),
                                   OrderBookedDate = this.PreProcess(rawRecord["OrderBookedDate"]),
                                   OrderAmendedDate = this.PreProcess(rawRecord["OrderAmendedDate"]),
                                   OrderRejectedDate = this.PreProcess(rawRecord["OrderRejectedDate"]),
                                   OrderCancelledDate = this.PreProcess(rawRecord["OrderCancelledDate"]),
                                   OrderFilledDate = this.PreProcess(rawRecord["OrderFilledDate"]),
                                   OrderBroker = this.PreProcess(rawRecord["OrderBroker"]),
                                   OrderType = this.PreProcess(rawRecord["OrderType"]),
                                   OrderDirection = this.PreProcess(rawRecord["OrderDirection"]),
                                   OrderCurrency = this.PreProcess(rawRecord["OrderCurrency"]),
                                   OrderSettlementCurrency = this.PreProcess(rawRecord["OrderSettlementCurrency"]),
                                   OrderCleanDirty = this.PreProcess(rawRecord["OrderCleanDirty"]),
                                   OrderAccumulatedInterest = this.PreProcess(rawRecord["OrderAccumulatedInterest"]),
                                   OrderLimitPrice = this.PreProcess(rawRecord["OrderLimitPrice"]),
                                   OrderAverageFillPrice = this.PreProcess(rawRecord["OrderAverageFillPrice"]),
                                   OrderOrderedVolume = this.PreProcess(rawRecord["OrderOrderedVolume"]),
                                   OrderFilledVolume = this.PreProcess(rawRecord["OrderFilledVolume"]),
                                   OrderTraderId = this.PreProcess(rawRecord["OrderTraderId"]),
                                   OrderTraderName = this.PreProcess(rawRecord["OrderTraderName"]),
                                   OrderClearingAgent = this.PreProcess(rawRecord["OrderClearingAgent"]),
                                   OrderDealingInstructions = this.PreProcess(rawRecord["OrderDealingInstructions"]),
                                   OrderOptionStrikePrice = this.PreProcess(rawRecord["OrderOptionStrikePrice"]),
                                   OrderOptionExpirationDate = this.PreProcess(rawRecord["OrderOptionExpirationDate"]),
                                   OrderOptionEuropeanAmerican =
                                       this.PreProcess(rawRecord["OrderOptionEuropeanAmerican"]),
                                   DealerOrderId = this.PreProcess(rawRecord["DealerOrderId"]),
                                   DealerOrderVersion = this.PreProcess(rawRecord["DealerOrderVersion"]),
                                   DealerOrderVersionLinkId = this.PreProcess(rawRecord["DealerOrderVersionLinkId"]),
                                   DealerOrderGroupId = this.PreProcess(rawRecord["DealerOrderGroupId"]),
                                   DealerOrderPlacedDate = this.PreProcess(rawRecord["DealerOrderPlacedDate"]),
                                   DealerOrderBookedDate = this.PreProcess(rawRecord["DealerOrderBookedDate"]),
                                   DealerOrderAmendedDate = this.PreProcess(rawRecord["DealerOrderAmendedDate"]),
                                   DealerOrderRejectedDate = this.PreProcess(rawRecord["DealerOrderRejectedDate"]),
                                   DealerOrderCancelledDate = this.PreProcess(rawRecord["DealerOrderCancelledDate"]),
                                   DealerOrderFilledDate = this.PreProcess(rawRecord["DealerOrderFilledDate"]),
                                   DealerOrderDealerId = this.PreProcess(rawRecord["DealerOrderDealerId"]),
                                   DealerOrderDealerName = this.PreProcess(rawRecord["DealerOrderDealerName"]),
                                   DealerOrderNotes = this.PreProcess(rawRecord["DealerOrderNotes"]),
                                   DealerOrderCounterParty = this.PreProcess(rawRecord["DealerOrderCounterParty"]),
                                   DealerOrderType = this.PreProcess(rawRecord["DealerOrderType"]),
                                   DealerOrderDirection = this.PreProcess(rawRecord["DealerOrderDirection"]),
                                   DealerOrderCurrency = this.PreProcess(rawRecord["DealerOrderCurrency"]),
                                   DealerOrderSettlementCurrency =
                                       this.PreProcess(rawRecord["DealerOrderSettlementCurrency"]),
                                   DealerOrderCleanDirty = this.PreProcess(rawRecord["DealerOrderCleanDirty"]),
                                   DealerOrderAccumulatedInterest =
                                       this.PreProcess(rawRecord["DealerOrderAccumulatedInterest"]),
                                   DealerOrderLimitPrice = this.PreProcess(rawRecord["DealerOrderLimitPrice"]),
                                   DealerOrderAverageFillPrice =
                                       this.PreProcess(rawRecord["DealerOrderAverageFillPrice"]),
                                   DealerOrderOrderedVolume = this.PreProcess(rawRecord["DealerOrderOrderedVolume"]),
                                   DealerOrderFilledVolume = this.PreProcess(rawRecord["DealerOrderFilledVolume"]),
                                   DealerOrderOptionStrikePrice =
                                       this.PreProcess(rawRecord["DealerOrderOptionStrikePrice"]),
                                   DealerOrderOptionExpirationDate =
                                       this.PreProcess(rawRecord["DealerOrderOptionExpirationDate"]),
                                   DealerOrderOptionEuropeanAmerican =
                                       this.PreProcess(rawRecord["DealerOrderOptionEuropeanAmerican"]),
                                   RowId = rowId
                               };

            this.Logger.LogInformation("Upload Trade File Processor has mapped raw record to csv dto");
            return tradeCsv;
        }
    }
}