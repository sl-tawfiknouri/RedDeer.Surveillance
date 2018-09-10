using System;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.Universe;

namespace Surveillance.Tests.Universe
{
    [TestFixture]
    public class UniverseBuilderTests
    {
        private IRedDeerTradeFormatRepository _tradeRepository;
        private IReddeerTradeFormatToReddeerTradeFrameProjector _documentProjector;

        [SetUp]
        public void Setup()
        {
            _tradeRepository = A.Fake<IRedDeerTradeFormatRepository>();
            _documentProjector = A.Fake<IReddeerTradeFormatToReddeerTradeFrameProjector>();
        }

        [Test]
        public void Constructor_NullTradeRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseBuilder(null, _documentProjector));
        }

        [Test]
        public void Constructor_NullProjector_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseBuilder(_tradeRepository, null));
        }

        [Test]
        public void Summon_DoesNot_ReturnNull()
        {
            var builder = new UniverseBuilder(_tradeRepository, _documentProjector);

            var result = builder.Summon(null);

            Assert.IsNotNull(result);
        }
    }
}
