using System;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Tests.Universe
{
    [TestFixture]
    public class UniverseBuilderTests
    {
        private IRedDeerTradeFormatRepository _tradeRepository;
        private IReddeerTradeFormatToReddeerTradeFrameProjector _documentProjector;

        private IRedDeerMarketExchangeFormatRepository _equityMarketRepository;
        private IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector _equityMarketProjector;

        private IMarketOpenCloseEventManager _marketManager;

        [SetUp]
        public void Setup()
        {
            _tradeRepository = A.Fake<IRedDeerTradeFormatRepository>();
            _documentProjector = A.Fake<IReddeerTradeFormatToReddeerTradeFrameProjector>();
            _equityMarketRepository = A.Fake<IRedDeerMarketExchangeFormatRepository>();
            _equityMarketProjector = A.Fake<IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector>();
            _marketManager = A.Fake<IMarketOpenCloseEventManager>();
        }

        [Test]
        public void Constructor_NullTradeRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UniverseBuilder(
                    null,
                    _documentProjector,
                    _equityMarketRepository,
                    _equityMarketProjector,
                    _marketManager));
        }

        [Test]
        public void Constructor_NullProjector_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new UniverseBuilder(
                    _tradeRepository,
                    null, 
                    _equityMarketRepository,
                    _equityMarketProjector,
                    _marketManager));
        }

        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder =
                new UniverseBuilder(
                    _tradeRepository,
                    _documentProjector,
                    _equityMarketRepository,
                    _equityMarketProjector,
                    _marketManager);

            var result = builder.Summon(null);

            Assert.IsNotNull(result);
        }
    }
}
