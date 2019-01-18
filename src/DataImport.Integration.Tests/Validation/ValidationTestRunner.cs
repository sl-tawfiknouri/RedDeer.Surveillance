using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataImport.Integration.Tests.ObjectGraphs;
using DomainV2.Trading;
using FakeItEasy;
using NUnit.Framework;

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
                            && a.Instrument.Identifiers.ClientIdentifier == "1",

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
                             && a.Instrument.Identifiers.ClientIdentifier == "2",

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
                            && a.Instrument.Identifiers.ClientIdentifier == "3",


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
                             && a.Instrument.Identifiers.ClientIdentifier == "4",

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
                             && a.Instrument.Identifiers.ClientIdentifier == "5",

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
                    })
            };
        }

        [Test]
        public void RunAllValidationTests()
        {
            if (!_validationFiles.Any())
            {
                return;
            }

            var graph = new TradeOrderStreamManagerGraph();

            foreach (var file in _validationFiles)
            {
                // ensure file is in the right location
                Assert.IsTrue(File.Exists(file.Path));

                // now pass it through to the data import app
                var manager = graph.Build();

                var ordersList = new List<Order>();
                A.CallTo(() => graph.OrdersRepository.Create(A<Order>.Ignored)).Invokes(i => ordersList.Add((Order)i.Arguments[0]));

                var fileMonitor = manager.Initialise();

                var processFile = fileMonitor.ProcessFile(file.Path);
                
                Assert.AreEqual(processFile, file.Success);

                A.CallTo(() => graph.OrdersRepository.Create(A<Order>.Ignored)).MustHaveHappenedANumberOfTimesMatching(n => n == file.SuccessfulRows);

                // positive, any conditions
                foreach (var cond in file.RowAssertions)
                {
                    Assert.IsTrue(ordersList.Any(cond));
                }
            }
        }
    }
}
