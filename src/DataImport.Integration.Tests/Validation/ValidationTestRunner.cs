using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataImport.Disk_IO.TradeFile;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Services.Interfaces;
using Domain.Core.Financial;
using Domain.Core.Financial.Assets;
using Domain.Core.Trading.Orders;
using FakeItEasy;
using Infrastructure.Network.Disk_IO;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;

namespace DataImport.Integration.Tests.Validation
{
    [TestFixture]
    public class ValidationTestRunner
    {
        private readonly IReadOnlyCollection<ValidationFile> _validationFiles;
        public ValidationTestRunner()
        {
            var subPath = Path.Combine("Validation", "Files");
            var filesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, subPath);

            _validationFiles = new List<ValidationFile>
            {
                new ValidationFile(
                    Path.Combine(filesDirectory, "tradevalidation1success.csv"),
                    true,
                    6,
                    new List<Func<Order, bool>>
                    {
                        a => a.OrderId == "943191f1-4144-4b0a-8a48-3c00a2ee20e7" 
                             && (int)a.Market.Type == 1
                            && a.Market.MarketIdentifierCode == "XLON"
                            && a.Market.Name == "London Stock Exchange"
                            && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                            && a.Instrument.Cfi == "entspb"
                            && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                            && a.Instrument.Identifiers.Sedol == "3163836"
                            && a.Instrument.Identifiers.Isin == "GB0031638363"
                            && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                            && a.Instrument.Identifiers.Cusip == "G4911B108"
                            && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                            && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                            && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                            && a.Instrument.Identifiers.ClientIdentifier == "1"
                            && a.PlacedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.BookedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.AmendedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.RejectedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.CancelledDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.FilledDate == new DateTime(2018, 01, 12, 9, 0, 0)
                            && a.OrderType == OrderTypes.LIMIT
                            && a.OrderDirection == OrderDirections.SELL
                            && a.OrderCurrency.Code == "GBP"
                            && a.OrderSettlementCurrency.Value.Code == "USD"
                            && a.OrderCleanDirty == OrderCleanDirty.CLEAN
                            && a.OrderAccumulatedInterest == 1
                            && a.OrderLimitPrice.Value.Value == 54.768m
                            && a.OrderAverageFillPrice.Value.Value == 56m
                            && a.OrderOrderedVolume.Value == 278
                            && a.OrderFilledVolume.Value == 278
                            && a.OrderTraderId == "RYAN"
                            && a.OrderTraderName == "Ryan Trenchard"
                            && a.OrderClearingAgent == "GOLDMAN"
                            && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                            && a.OrderOptionStrikePrice.Value.Value == 100
                            && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                            && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.EUROPEAN
                            && a.DealerOrders.Count == 1
                            && a.DealerOrders.First().DealerOrderId == "100b"
                            && a.DealerOrders.First().PlacedDate == new DateTime(2018, 01, 18, 9, 0, 0)
                             && a.DealerOrders.First().BookedDate == new DateTime(2018, 01, 18, 9, 0, 0)
                             && a.DealerOrders.First().AmendedDate == new DateTime(2018, 01, 18, 9, 0, 0)
                             && a.DealerOrders.First().RejectedDate == null
                             && a.DealerOrders.First().CancelledDate == null
                             && a.DealerOrders.First().FilledDate == new DateTime(2018, 01, 18, 9, 0, 0)
                             && a.DealerOrders.First().DealerId == "RYAN"
                            && a.DealerOrders.First().DealerName == "Ryan Trenchard"
                            && a.DealerOrders.First().Notes == "Trade within 1% of vwap"
                            && a.DealerOrders.First().DealerCounterParty == "Barclays Capital"
                            && a.DealerOrders.First().OrderType == OrderTypes.MARKET
                            && a.DealerOrders.First().OrderDirection == OrderDirections.BUY
                            && a.DealerOrders.First().Currency.Code == "GBP"
                            && a.DealerOrders.First().SettlementCurrency.Code == "CNY"
                            && a.DealerOrders.First().CleanDirty == OrderCleanDirty.CLEAN
                            && a.DealerOrders.First().AccumulatedInterest == 1
                            && a.DealerOrders.First().LimitPrice.Value.Value == 10m
                            && a.DealerOrders.First().AverageFillPrice.Value.Value == 100m
                            && a.DealerOrders.First().OrderedVolume == 50
                            && a.DealerOrders.First().FilledVolume == 200
                            && a.DealerOrders.First().OptionStrikePrice == 30
                            && a.DealerOrders.First().OptionExpirationDate == new DateTime(2018, 01, 18, 9, 0, 0)
                            && a.DealerOrders.First().OptionEuropeanAmerican == OptionEuropeanAmerican.AMERICAN,

                        a => a.OrderId == "9e03994a-c681-4851-b0fa-0da721c0a880"
                             && (int)a.Market.Type == 1
                             && a.Market.MarketIdentifierCode == "XLON"
                             && a.Market.Name == "London Stock Exchange"
                             && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                             && a.Instrument.Cfi == "entspb"
                             && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                             && a.Instrument.Identifiers.Sedol == "3163836"
                             && a.Instrument.Identifiers.Isin == "GB0031638363"
                             && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                             && a.Instrument.Identifiers.Cusip == "G4911B108"
                             && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                             && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                             && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                             && a.Instrument.Identifiers.ClientIdentifier == "2"
                             && a.PlacedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.BookedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.AmendedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.RejectedDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.CancelledDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.FilledDate == new DateTime(2018, 01, 12, 9, 0, 0)
                             && a.OrderType == OrderTypes.LIMIT
                             && a.OrderDirection == OrderDirections.SELL
                             && a.OrderCurrency.Code == "GBP"
                             && a.OrderSettlementCurrency.Value.Code == "USD"
                             && a.OrderCleanDirty == OrderCleanDirty.CLEAN
                             && a.OrderAccumulatedInterest == 2
                             && a.OrderLimitPrice.Value.Value == 54.768m
                             && a.OrderAverageFillPrice.Value.Value == 56m
                             && a.OrderOrderedVolume.Value == 278
                             && a.OrderFilledVolume.Value == 278
                             && a.OrderTraderId == "RYAN"
                             && a.OrderTraderName == "Ryan Trenchard"
                             && a.OrderClearingAgent == "GOLDMAN"
                             && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                             && a.OrderOptionStrikePrice.Value.Value == 200
                             && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                             && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.EUROPEAN,


                        a => a.OrderId == "e21fd385-c940-46fa-83c8-570b1f0207f3"
                             && (int)a.Market.Type == 1
                            && a.Market.MarketIdentifierCode == "XLON"
                            && a.Market.Name == "London Stock Exchange"
                            && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                            && a.Instrument.Cfi == "entspb"
                            && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                            && a.Instrument.Identifiers.Sedol == "3163836"
                            && a.Instrument.Identifiers.Isin == "GB0031638363"
                            && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                            && a.Instrument.Identifiers.Cusip == "G4911B108"
                            && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                            && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                            && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                            && a.Instrument.Identifiers.ClientIdentifier == "3"
                             && a.PlacedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.BookedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.AmendedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.RejectedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.CancelledDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.FilledDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.OrderType == OrderTypes.LIMIT
                             && a.OrderDirection == OrderDirections.SELL
                             && a.OrderCurrency.Code == "GBP"
                             && a.OrderSettlementCurrency.Value.Code == "USD"
                             && a.OrderCleanDirty == OrderCleanDirty.CLEAN
                             && a.OrderAccumulatedInterest == 3
                             && a.OrderLimitPrice.Value.Value == 54.768m
                             && a.OrderAverageFillPrice.Value.Value == 56m
                             && a.OrderOrderedVolume.Value == 278
                             && a.OrderFilledVolume.Value == 278
                             && a.OrderTraderId == "RYAN"
                             && a.OrderTraderName == "Ryan Trenchard"
                             && a.OrderClearingAgent == "GOLDMAN"
                             && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                             && a.OrderOptionStrikePrice.Value.Value == 300
                             && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                             && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.EUROPEAN,


                        a => a.OrderId == "11ced167-82ac-46de-bc78-11dadf689033"
                             && (int)a.Market.Type == 1
                             && a.Market.MarketIdentifierCode == "XLON"
                             && a.Market.Name == "London Stock Exchange"
                             && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                             && a.Instrument.Cfi == "entspb"
                             && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                             && a.Instrument.Identifiers.Sedol == "3163836"
                             && a.Instrument.Identifiers.Isin == "GB0031638363"
                             && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                             && a.Instrument.Identifiers.Cusip == "G4911B108"
                             && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                             && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                             && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                             && a.Instrument.Identifiers.ClientIdentifier == "4"
                             && a.PlacedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.BookedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.AmendedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.RejectedDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.CancelledDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.FilledDate == new DateTime(2018, 01, 15, 9, 0, 0)
                             && a.OrderType == OrderTypes.LIMIT
                             && a.OrderDirection == OrderDirections.SELL
                             && a.OrderCurrency.Code == "GBP"
                             && a.OrderSettlementCurrency.Value.Code == "USD"
                             && a.OrderCleanDirty == OrderCleanDirty.DIRTY
                             && a.OrderAccumulatedInterest == 4
                             && a.OrderLimitPrice.Value.Value == 54.768m
                             && a.OrderAverageFillPrice.Value.Value == 56m
                             && a.OrderOrderedVolume.Value == 278
                             && a.OrderFilledVolume.Value == 278
                             && a.OrderTraderId == "RYAN"
                             && a.OrderTraderName == "Ryan Trenchard"
                             && a.OrderClearingAgent == "GOLDMAN"
                             && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                             && a.OrderOptionStrikePrice.Value.Value == 400
                             && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                             && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.AMERICAN,

                        a => a.OrderId == "f1aadd18-d923-4a9f-9d1e-3c5e60dad40a"
                             && (int)a.Market.Type == 1
                             && a.Market.MarketIdentifierCode == "XLON"
                             && a.Market.Name == "London Stock Exchange"
                             && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                             && a.Instrument.Cfi == "entspb"
                             && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                             && a.Instrument.Identifiers.Sedol == "3163836"
                             && a.Instrument.Identifiers.Isin == "GB0031638363"
                             && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                             && a.Instrument.Identifiers.Cusip == "G4911B108"
                             && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                             && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                             && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                             && a.Instrument.Identifiers.ClientIdentifier == "5"
                             && a.PlacedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.BookedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.AmendedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.RejectedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.CancelledDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.FilledDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.OrderType == OrderTypes.LIMIT
                             && a.OrderDirection == OrderDirections.SELL
                             && a.OrderCurrency.Code == "GBP"
                             && a.OrderSettlementCurrency.Value.Code == "USD"
                             && a.OrderCleanDirty == OrderCleanDirty.DIRTY
                             && a.OrderAccumulatedInterest == 5
                             && a.OrderLimitPrice.Value.Value == 54.768m
                             && a.OrderAverageFillPrice.Value.Value == 56m
                             && a.OrderOrderedVolume.Value == 278
                             && a.OrderFilledVolume.Value == 278
                             && a.OrderTraderId == "RYAN"
                             && a.OrderTraderName == "Ryan Trenchard"
                             && a.OrderClearingAgent == "GOLDMAN"
                             && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                             && a.OrderOptionStrikePrice.Value.Value == 500
                             && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                             && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.AMERICAN,

                        a => a.OrderId == "6afdc02d-c1fd-4782-ab8a-9fd97ac4b698"
                             && (int)a.Market.Type == 1
                             && a.Market.MarketIdentifierCode == "XLON"
                             && a.Market.Name == "London Stock Exchange"
                             && a.Instrument.Name == "INTERTEK GROUP PLC  ORD"
                             && a.Instrument.Cfi == "entspb"
                             && a.Instrument.IssuerIdentifier == "Intertek Group Plc"
                             && a.Instrument.Identifiers.Sedol == "3163836"
                             && a.Instrument.Identifiers.Isin == "GB0031638363"
                             && a.Instrument.Identifiers.Figi == "BBG000JQZV13"
                             && a.Instrument.Identifiers.Cusip == "G4911B108"
                             && a.Instrument.Identifiers.Lei == "8156007259ABDEA3F444"
                             && a.Instrument.Identifiers.ExchangeSymbol == "ITRK"
                             && a.Instrument.Identifiers.BloombergTicker == "ITRK LN Equity"
                             && a.Instrument.Identifiers.ClientIdentifier == "6"
                             && a.PlacedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.BookedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.AmendedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.RejectedDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.CancelledDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.FilledDate == new DateTime(2018, 01, 16, 9, 0, 0)
                             && a.OrderType == OrderTypes.LIMIT
                             && a.OrderDirection == OrderDirections.SELL
                             && a.OrderCurrency.Code == "GBP"
                             && a.OrderSettlementCurrency.Value.Code == "USD"
                             && a.OrderCleanDirty == OrderCleanDirty.DIRTY
                             && a.OrderAccumulatedInterest == 6
                             && a.OrderLimitPrice.Value.Value == 54.768m
                             && a.OrderAverageFillPrice.Value.Value == 56m
                             && a.OrderOrderedVolume.Value == 278
                             && a.OrderFilledVolume.Value == 278
                             && a.OrderTraderId == "RYAN"
                             && a.OrderTraderName == "Ryan Trenchard"
                             && a.OrderClearingAgent == "GOLDMAN"
                             && a.OrderDealingInstructions == "DEAL 1% OF VWAP"
                             && a.OrderOptionStrikePrice.Value.Value == 600
                             && a.OrderOptionExpirationDate.Value == new DateTime(2018, 01, 18, 09, 0, 0)
                             && a.OrderOptionEuropeanAmerican == OptionEuropeanAmerican.AMERICAN
                    })
            };
        }

        private IEnrichmentService _enrichmentService;
        private IOrdersRepository _ordersRepository;
        private IFileUploadOrdersRepository _fileUploadOrdersRepository;
        private IUploadCoordinatorMessageSender _uploadMessageSender;
        private ISystemProcessContext _systemProcessContext;

        private UploadTradeFileMonitor _uploadTradeFileMonitor;

        [SetUp]
        public void Setup()
        {
            _enrichmentService = A.Fake<IEnrichmentService>();
            _ordersRepository = A.Fake<IOrdersRepository>();
            _fileUploadOrdersRepository = A.Fake<IFileUploadOrdersRepository>();
            _systemProcessContext = A.Fake<ISystemProcessContext>();
            _uploadMessageSender = A.Fake<IUploadCoordinatorMessageSender>();

            _uploadTradeFileMonitor = new UploadTradeFileMonitor(
                new Configuration.Configuration(),
                new ReddeerDirectory(),
                new UploadTradeFileProcessor(
                    new OrderFileToOrderSerialiser(),
                    new OrderFileValidator(),
                    new NullLogger<UploadTradeFileProcessor>()),
                _enrichmentService,
                _ordersRepository,
                _fileUploadOrdersRepository,
                _uploadMessageSender,
                _systemProcessContext,
                new NullLogger<UploadTradeFileMonitor>());
        }

        [Test]
        public void RunAllValidationTests()
        {
            if (!_validationFiles.Any())
            {
                return;
            }

            foreach (var file in _validationFiles)
            {
                // ensure file is in the right location
                Assert.IsTrue(File.Exists(file.Path));

                var ordersList = new List<Order>();
                A.CallTo(() => _ordersRepository.Create(A<Order>.Ignored))
                    .Invokes(i => ordersList.Add((Order)i.Arguments[0]));

                var processFile = _uploadTradeFileMonitor.ProcessFile(file.Path);

                Assert.AreEqual(processFile, file.Success);

                A.CallTo(() => _ordersRepository.Create(A<Order>.Ignored))
                    .MustHaveHappenedANumberOfTimesMatching(n => n == file.SuccessfulRows);

                // positive, any conditions
                foreach (var cond in file.RowAssertions)
                {
                    Assert.IsTrue(ordersList.Any(cond));
                }
            }
        }
    }
}
