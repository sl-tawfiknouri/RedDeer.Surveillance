using System;
using System.Linq;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Markets;

namespace Surveillance.Tests.Markets
{
    [TestFixture]
    public class UniverseMarketCacheTests
    {
        private IBmllDataRequestRepository _requestRepository;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _requestRepository = A.Fake<IBmllDataRequestRepository>();
            _logger = A.Fake<ILogger>();
        }

        [Test]
        public void Add_ExchangeFrame_OutOfDateRange_CallsDataRequestRepository()
        {
            var cache = new UniverseMarketCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
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

            var securityTick = new SecurityTick(
                security,
                new Spread(
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp")),
                new Volume(1000),
                new Volume(2000),
                DateTime.Now.AddDays(1),
                10000,
                new IntradayPrices(null, null, null, null),
                15,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new ExchangeFrame(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.Now.AddDays(1),
                new SecurityTick[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", securityIdentifiers, DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now, "0");

            cache.Add(frame);

            var result = cache.Get(marketData).Response;

            Assert.IsNull(result);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_InDateRange_DoesNotCall()
        {
            var cache = new UniverseMarketCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
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

            var securityTick = new SecurityTick(
                security,
                new Spread(
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp")),
                new Volume(1000),
                new Volume(2000),
                DateTime.Now,
                10000,
                new IntradayPrices(null, null, null, null),
                15,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new ExchangeFrame(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.Now,
                new SecurityTick[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", securityIdentifiers, DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now, "0");

            cache.Add(frame);

            var result = cache.Get(marketData).Response;

            Assert.AreEqual(result, securityTick);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustNotHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_OutOfDateRange_GetMarkets_CallsDataRequestRepository()
        {
            var cache = new UniverseMarketCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
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

            var securityTick = new SecurityTick(
                security,
                new Spread(
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp")),
                new Volume(1000),
                new Volume(2000),
                DateTime.Now.AddDays(1),
                10000,
                new IntradayPrices(null, null, null, null),
                15,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new ExchangeFrame(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.Now.AddDays(1),
                new SecurityTick[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", securityIdentifiers, DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now, "0");
            cache.Add(frame);

            var result = cache.GetMarkets(marketData).Response;

            Assert.IsNull(result);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustHaveHappened();
        }

        [Test]
        public void Add_ExchangeFrame_InDateRange_GetMarkets_DoesNotCall()
        {
            var cache = new UniverseMarketCache(TimeSpan.FromMinutes(15), _requestRepository, _logger);

            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
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

            var securityTick = new SecurityTick(
                security,
                new Spread(
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp"),
                    new CurrencyAmount(0, "gbp")),
                new Volume(1000),
                new Volume(2000),
                DateTime.Now,
                10000,
                new IntradayPrices(null, null, null, null),
                15,
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE));

            var frame = new ExchangeFrame(
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                DateTime.Now,
                new SecurityTick[]
                {
                    securityTick
                });

            var marketData = new MarketDataRequest("XLON", securityIdentifiers, DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now, "0");

            cache.Add(frame);

            var result = cache.GetMarkets(marketData).Response;

            Assert.AreEqual(result.FirstOrDefault(), securityTick);
            A.CallTo(() => _requestRepository.CreateDataRequest(marketData)).MustNotHaveHappened();
        }
    }
}
