using System.Collections.Generic;
using DataImport.Disk_IO.EtlFile;
using FluentValidation.Results;
using NUnit.Framework;
using SharedKernel.Files.Orders;

namespace DataImport.Tests.Disk_IO.EtlFile
{
    [TestFixture]
    public class EtlUploadErrorStoreTests
    {
        [Test]
        public void AddNullContract_SerialisedErrors_Empty()
        {
            var store = new EtlUploadErrorStore();
            var validationFailures = new List<ValidationFailure>();

            store.Add(null, validationFailures);
            var errors = store.SerialisedErrors();

            Assert.AreEqual(errors, string.Empty);
        }

        [Test]
        public void AddThenClear_SerialisedErrors_IsEmpty()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract();
            var validationFailures = new List<ValidationFailure>();

            store.Add(contract, validationFailures);
            store.Clear();

            var errors = store.SerialisedErrors();

            Assert.AreEqual(errors, string.Empty);
        }

        [Test]
        public void Add_SerialisedErrors_ExpectedResponse()
        {
            var store = new EtlUploadErrorStore();
            var contract = new OrderFileContract();
            var validationFailures = new List<ValidationFailure>();

            store.Add(contract, validationFailures);
            var errors = store.SerialisedErrors();
            var expectedResult =
                " {\"MarketType\":null,\"MarketIdentifierCode\":null,\"MarketName\":null,\"InstrumentName\":null,\"InstrumentCfi\":null,\"InstrumentIssuerIdentifier\":null,\"InstrumentClientIdentifier\":null,\"InstrumentSedol\":null,\"InstrumentIsin\":null,\"InstrumentFigi\":null,\"InstrumentCusip\":null,\"InstrumentLei\":null,\"InstrumentExchangeSymbol\":null,\"InstrumentBloombergTicker\":null,\"InstrumentUnderlyingName\":null,\"InstrumentUnderlyingCfi\":null,\"InstrumentUnderlyingIssuerIdentifier\":null,\"InstrumentUnderlyingClientIdentifier\":null,\"InstrumentUnderlyingSedol\":null,\"InstrumentUnderlyingIsin\":null,\"InstrumentUnderlyingFigi\":null,\"InstrumentUnderlyingCusip\":null,\"InstrumentUnderlyingLei\":null,\"InstrumentUnderlyingExchangeSymbol\":null,\"InstrumentUnderlyingBloombergTicker\":null,\"OrderId\":null,\"OrderVersion\":null,\"OrderVersionLinkId\":null,\"OrderGroupId\":null,\"OrderPlacedDate\":null,\"OrderBookedDate\":null,\"OrderAmendedDate\":null,\"OrderRejectedDate\":null,\"OrderCancelledDate\":null,\"OrderFilledDate\":null,\"OrderType\":null,\"OrderDirection\":null,\"OrderCurrency\":null,\"OrderSettlementCurrency\":null,\"OrderCleanDirty\":null,\"OrderAccumulatedInterest\":null,\"OrderLimitPrice\":null,\"OrderAverageFillPrice\":null,\"OrderOrderedVolume\":null,\"OrderFilledVolume\":null,\"OrderTraderId\":null,\"OrderTraderName\":null,\"OrderClearingAgent\":null,\"OrderDealingInstructions\":null,\"OrderOptionStrikePrice\":null,\"OrderOptionExpirationDate\":null,\"OrderOptionEuropeanAmerican\":null,\"DealerOrderId\":null,\"DealerOrderVersion\":null,\"DealerOrderVersionLinkId\":null,\"DealerOrderGroupId\":null,\"DealerOrderPlacedDate\":null,\"DealerOrderBookedDate\":null,\"DealerOrderAmendedDate\":null,\"DealerOrderRejectedDate\":null,\"DealerOrderCancelledDate\":null,\"DealerOrderFilledDate\":null,\"DealerOrderDealerId\":null,\"DealerOrderDealerName\":null,\"DealerOrderNotes\":null,\"DealerOrderCounterParty\":null,\"DealerOrderType\":null,\"DealerOrderDirection\":null,\"DealerOrderCurrency\":null,\"DealerOrderSettlementCurrency\":null,\"DealerOrderCleanDirty\":null,\"DealerOrderAccumulatedInterest\":null,\"DealerOrderLimitPrice\":null,\"DealerOrderAverageFillPrice\":null,\"DealerOrderOrderedVolume\":null,\"DealerOrderFilledVolume\":null,\"DealerOrderOptionStrikePrice\":null,\"DealerOrderOptionExpirationDate\":null,\"DealerOrderOptionEuropeanAmerican\":null,\"RowId\":0} \r\n  \r\n \r\n";

            Assert.AreEqual(errors, expectedResult);
        }
    }
}
