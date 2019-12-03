namespace Surveillance.Engine.Rules.Tests.Markets
{
    using System;

    using FakeItEasy;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    [TestFixture]
    public class InterdayMarketCacheStrategyTests
    {
        private IUniverseEquityInterDayCache _cache;

        [Test]
        public void Constructor_Cache_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityInterDayMarketCacheStrategy(null));
        }

        [Test]
        public void Query_Calls_Cache()
        {
            var cacheStrategy = new EquityInterDayMarketCacheStrategy(this._cache);
            var dataRequest = MarketDataRequest.Null();

            cacheStrategy.Query(dataRequest);

            A.CallTo(() => this._cache.Get(dataRequest)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Query_Returns_Interday_Market_Data_Response()
        {
            var cacheStrategy = new EquityInterDayMarketCacheStrategy(this._cache);
            var dataRequest = MarketDataRequest.Null();

            var response = cacheStrategy.Query(dataRequest);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<EquityInterDayMarketDataResponse>(response);
        }

        [SetUp]
        public void Setup()
        {
            this._cache = A.Fake<IUniverseEquityInterDayCache>();
        }
    }
}