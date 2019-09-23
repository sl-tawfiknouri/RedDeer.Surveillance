namespace Surveillance.Engine.Rules.Tests.Markets
{
    using FakeItEasy;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    [TestFixture]
    public class MarketDataCacheStrategyFactoryTests
    {
        private IUniverseEquityInterDayCache _interdayCache;

        private IUniverseEquityIntradayCache _intradayCache;

        [Test]
        public void InterdayStrategy_Returns_A_InterdayMarketCacheStrategy()
        {
            var strategyFactory = this.Build();

            var result = strategyFactory.InterdayStrategy(this._interdayCache);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InterdayMarketCacheStrategy>(result);
        }

        [Test]
        public void IntradayStrategy_Returns_A_IntradayMarketCacheStrategy()
        {
            var strategyFactory = this.Build();

            var result = strategyFactory.IntradayStrategy(this._intradayCache);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IntradayMarketCacheStrategy>(result);
        }

        [SetUp]
        public void Setup()
        {
            this._interdayCache = A.Fake<IUniverseEquityInterDayCache>();
            this._intradayCache = A.Fake<IUniverseEquityIntradayCache>();
        }

        private IMarketDataCacheStrategyFactory Build()
        {
            return new MarketDataCacheStrategyFactory();
        }
    }
}