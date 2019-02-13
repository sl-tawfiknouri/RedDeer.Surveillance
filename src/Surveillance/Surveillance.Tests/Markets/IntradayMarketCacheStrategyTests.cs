using System;
using DomainV2.Markets;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Tests.Markets
{
    [TestFixture]
    public class IntradayMarketCacheStrategyTests
    {
        private IUniverseEquityIntradayCache _cache;

        [SetUp]
        public void Setup()
        {
            _cache = A.Fake<IUniverseEquityIntradayCache>();
        }

        [Test]
        public void Constructor_Cache_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new IntradayMarketCacheStrategy(null));
        }

        [Test]
        public void Query_Request_Passed_To_Underlying_Cache()
        {
            var strategy = new IntradayMarketCacheStrategy(_cache);
            var marketDataRequest = MarketDataRequest.Null();

            var result = strategy.Query(marketDataRequest);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IntradayMarketDataResponse>(result);
            A
                .CallTo(() => _cache.GetForLatestDayOnly(marketDataRequest))
                .MustHaveHappenedOnceExactly();
        }
    }
}
