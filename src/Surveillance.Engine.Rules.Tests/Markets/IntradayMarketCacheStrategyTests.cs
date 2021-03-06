﻿namespace Surveillance.Engine.Rules.Tests.Markets
{
    using System;

    using FakeItEasy;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    [TestFixture]
    public class IntradayMarketCacheStrategyTests
    {
        private IUniverseEquityIntraDayCache _cache;

        [Test]
        public void Constructor_Cache_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new EquityIntraDayMarketCacheStrategy(null));
        }

        [Test]
        public void Query_Request_Passed_To_Underlying_Cache()
        {
            var strategy = new EquityIntraDayMarketCacheStrategy(this._cache);
            var marketDataRequest = MarketDataRequest.Null();

            var result = strategy.Query(marketDataRequest);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<EquityIntraDayMarketDataResponse>(result);
            A.CallTo(() => this._cache.GetForLatestDayOnly(marketDataRequest)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._cache = A.Fake<IUniverseEquityIntraDayCache>();
        }
    }
}