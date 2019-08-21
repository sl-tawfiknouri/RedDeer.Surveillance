namespace Domain.Core.Tests.Markets
{
    using Domain.Core.Markets;

    using NUnit.Framework;

    [TestFixture]
    public class MarketTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var id = "100";
            var mic = "XLON";
            var name = "London Stock Exchange";
            var type = MarketTypes.DarkPool;

            var market = new Market(id, mic, name, type);

            Assert.AreEqual(id, market.Id);
            Assert.AreEqual(mic, market.MarketIdentifierCode);
            Assert.AreEqual(name, market.Name);
            Assert.AreEqual(type, market.Type);
        }

        [Test]
        public void Ctor_DoesNotThrow_WithNullArgs()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new Market(null, null, null, MarketTypes.DarkPool));
        }
    }
}