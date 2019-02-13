using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Tests.Markets
{
    [TestFixture]
    public class MarketDataCacheStrategyFactoryTests
    {
        private IUniverseEquityInterDayCache _interdayCache;
        private IUniverseEquityIntradayCache _intradayCache;

        [SetUp]
        public void Setup()
        {
            _interdayCache = A.Fake<IUniverseEquityInterDayCache>();
            _intradayCache = A.Fake<IUniverseEquityIntradayCache>();
        }

        [Test]
        public void InterdayStrategy_Returns_A_InterdayMarketCacheStrategy()
        {
            var strategyFactory = Build();

            var result = strategyFactory.InterdayStrategy(_interdayCache);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InterdayMarketCacheStrategy>(result);
        }

        [Test]
        public void IntradayStrategy_Returns_A_IntradayMarketCacheStrategy()
        {
            var strategyFactory = Build();

            var result = strategyFactory.IntradayStrategy(_intradayCache);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IntradayMarketCacheStrategy>(result);
        }

        private IMarketDataCacheStrategyFactory Build()
        {
            return new MarketDataCacheStrategyFactory();
        }
    }
}
