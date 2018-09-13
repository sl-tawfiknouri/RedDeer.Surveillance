using System;
using System.Linq;
using NUnit.Framework;
using Surveillance.DataLayer.Projectors;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.Tests.Projectors
{
    [TestFixture]
    public class ReddeerMarketExchangeFormatToReddeerExchangeFormatProjectorTests
    {
        [Test]
        public void Project_NullMarketDocument_ReturnNullFrame()
        {
            var projector = new ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector();

            var result = projector.Project((ReddeerMarketDocument)null);

            Assert.IsNull(result);
        }

        [Test]
        public void Project_MarketDocumentWithNoSecurities_ReturnsExpectedResults()
        {
            var projector = new ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector();
            var marketDocument = new ReddeerMarketDocument { MarketId = "LSE", MarketName = "London Stock Exchange", DateTime = DateTime.Parse("01/12/2018")};
            
            var result = projector.Project(marketDocument);

            Assert.IsNotNull(result);                     
        }

        [Test]
        public void Project_MarketDocumentWithOneSecurity_ReturnsExpectedResults()
        {
            var projector = new ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector();

            var security = new ReddeerSecurityDocument
            {
                SecurityClientIdentifier = "Standard Chartered HK",
                SecuritySedol = "SC12345",
                SecurityIsin = "STA123456789",
                SecurityFigi = "",
                SecurityName = "Standard Chartered Class B Shares",
                SpreadBuy = 19.9m,
                SpreadBuyCurrency = "GBP",
                SpreadSell = 20m,
                SpreadSellCurrency = "GBP",
                SpreadPrice = 20m,
                SpreadPriceCurrency = "GBP",
                Volume = 20000,
                TimeStamp = DateTime.Parse("01/12/2018"),
                MarketCap = 400000
            };

            var marketDocument =
                new ReddeerMarketDocument
                {
                    MarketId = "LSE",
                    MarketName = "London Stock Exchange",
                    DateTime = DateTime.Parse("01/12/2018"),
                    Securities = new [] { security }
                };

            var result = projector.Project(marketDocument);

            Assert.IsNotNull(result);
            Assert.AreEqual("LSE", result.Exchange.Id.Id);
            Assert.AreEqual("London Stock Exchange", result.Exchange.Name);
            Assert.AreEqual(1, result.Securities.Count);

            var firstSecurity = result.Securities.First();

            Assert.AreEqual(400000m, firstSecurity.MarketCap);
            Assert.AreEqual(null, firstSecurity.TickerSymbol);
            Assert.AreEqual(null, firstSecurity.CfiCode);
            Assert.AreEqual(20000m, firstSecurity.Volume.Traded);
            Assert.AreEqual(DateTime.Parse("01/12/2018"), firstSecurity.TimeStamp);

            Assert.AreEqual("GBP", firstSecurity.Spread.Price.Currency);
            Assert.AreEqual(20m, firstSecurity.Spread.Price.Value);

            Assert.AreEqual("GBP", firstSecurity.Spread.Bid.Currency);
            Assert.AreEqual(19.9m, firstSecurity.Spread.Bid.Value);

            Assert.AreEqual("GBP", firstSecurity.Spread.Ask.Currency);
            Assert.AreEqual(20m, firstSecurity.Spread.Ask.Value);

            Assert.AreEqual("Standard Chartered Class B Shares", firstSecurity.Security.Name);
            Assert.AreEqual("Standard Chartered HK", firstSecurity.Security.Identifiers.ClientIdentifier);
            Assert.AreEqual("SC12345", firstSecurity.Security.Identifiers.Sedol);
            Assert.AreEqual("STA123456789", firstSecurity.Security.Identifiers.Isin);
            Assert.AreEqual("", firstSecurity.Security.Identifiers.Figi);
        }
    }
}
