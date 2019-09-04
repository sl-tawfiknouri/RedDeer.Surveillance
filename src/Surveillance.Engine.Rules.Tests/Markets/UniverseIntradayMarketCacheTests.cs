using System;
using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using SharedKernel.Contracts.Markets;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Markets;

namespace Surveillance.Engine.Rules.Tests.Markets
{
    [TestFixture]
    public class UniverseIntradayMarketCacheTests
    {
        private IRuleRunDataRequestRepository _requestRepository;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _requestRepository = A.Fake<IRuleRunDataRequestRepository>();
            _logger = A.Fake<ILogger>();
        }

        [Test]
        public void Add_ExchangeFrame_OutOfDateRange_CallsDataRequestRepository()
        {
            var cache = new UniverseEquityIntradayCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
                    string.Empty,
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi",
                    "cusip",
                    "test",
                    "test lei",
                    "ticker");

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var securityTick = new EquityInstrumentIntraDayTimeBar(
                security,
                new SpreadTimeBar(
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Volume(1000)),
                new DailySummaryTimeBar(
                    10000,
                    "USD",
                    new IntradayPrices(null, null, null, null),
                    1000,
                    new Volume(2000),
                    DateTime.UtcNow.AddDays(1)
                    ), 
                DateTime.UtcNow.AddDays(1),
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow.AddDays(1),
                new EquityInstrumentIntraDayTimeBar[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", "e", securityIdentifiers, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), DateTime.UtcNow, "0", DataSource.AllIntraday);

            cache.Add(frame);

            var result = cache.GetForLatestDayOnly(marketData).Response;

            Assert.IsNull(result);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_InDateRange_DoesNotCall()
        {
            var cache = new UniverseEquityIntradayCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
                    string.Empty,
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi",
                    "cusip",
                    "test",
                    "test lei",
                    "ticker");

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var securityTick = new EquityInstrumentIntraDayTimeBar(
                security,
                new SpreadTimeBar(
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Volume(1000)),
                new DailySummaryTimeBar(
                    10000,
                    "USD",
                    new IntradayPrices(null, null, null, null),
                    1000,
                    new Volume(1000),
                    DateTime.UtcNow), 
                DateTime.UtcNow,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", "e", securityIdentifiers, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), DateTime.UtcNow, "0", DataSource.AllIntraday);

            cache.Add(frame);

            var result = cache.GetForLatestDayOnly(marketData).Response;

            Assert.AreEqual(result, securityTick);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustNotHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_OutOfDateRange_GetMarkets_CallsDataRequestRepository()
        {
            var cache = new UniverseEquityIntradayCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
                    string.Empty,
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi",
                    "cusip",
                    "test",
                    "test lei",
                    "ticker");

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var securityTick = new EquityInstrumentIntraDayTimeBar(
                security,
                new SpreadTimeBar(
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Volume(1000)),
                new DailySummaryTimeBar(
                    1000,
                    "USD",
                    new IntradayPrices(null, null, null, null),
                    10000,
                    new Volume(2000),
                    DateTime.UtcNow.AddDays(1)), 
                DateTime.UtcNow.AddDays(1),
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow.AddDays(1),
                new EquityInstrumentIntraDayTimeBar[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", "e", securityIdentifiers, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), DateTime.UtcNow, "0", DataSource.AllIntraday);
            cache.Add(frame);

            var result = cache.GetMarkets(marketData).Response;

            Assert.IsNull(result);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_InDateRange_GetMarkets_DoesNotCall()
        {
            var cache = new UniverseEquityIntradayCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
                    string.Empty,
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi",
                    "cusip",
                    "test",
                    "test lei",
                    "ticker");

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var securityTick = new EquityInstrumentIntraDayTimeBar(
                security,
                new SpreadTimeBar(
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Money(0, "gbp"),
                    new Volume(1000)),
                new DailySummaryTimeBar(
                    1000,
                    "USD",
                    new IntradayPrices(null, null, null, null),
                    1000,
                    new Volume(2000),
                    DateTime.UtcNow
                    ),
                DateTime.UtcNow,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new EquityIntraDayTimeBarCollection(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", "e", securityIdentifiers, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), DateTime.UtcNow, "0", DataSource.AllIntraday);

            cache.Add(frame);

            var result = cache.GetMarkets(marketData).Response;

            Assert.AreEqual(result.FirstOrDefault(), securityTick);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustNotHaveHappened();
        }
    }
}
