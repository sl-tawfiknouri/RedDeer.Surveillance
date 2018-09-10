using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Projectors;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.Tests.Projectors
{
    [TestFixture]
    public class ReddeerTradeFormatToReddeerTradeFrameProjectorTests
    {
        private ILogger<ReddeerTradeFormatToReddeerTradeFrameProjector> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ReddeerTradeFormatToReddeerTradeFrameProjector>>();
        }

        [Test]
        public void Project_NullTradeDocuments_ReturnsEmptyList()
        {
            var projector = new ReddeerTradeFormatToReddeerTradeFrameProjector(_logger);

            var result = projector.Project(null);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Project_CollectionContainsNulls_ReturnsEmptyList()
        {
            var projector = new ReddeerTradeFormatToReddeerTradeFrameProjector(_logger);

            var result = projector.Project(new List<ReddeerTradeDocument> {null, null, null});

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Project_ProjectDocuments_AsExpected()
        {
            var projector = new ReddeerTradeFormatToReddeerTradeFrameProjector(_logger);
            var docs =
                new List<ReddeerTradeDocument>
                {
                    new ReddeerTradeDocument
                    {
                        MarketName = "LSE",
                        MarketId = "LSE",
                        Id = "123112",
                        Limit = 12,
                        SecurityId = "STAN",
                        SecurityName = "Standard Chartered",
                        OrderDirectionDescription = "Buy",
                        OrderDirectionId = 0,
                        OrderStatusDescription = "Cancelled",
                        OrderStatusId = 1,
                        OrderTypeId = 2,
                        OrderTypeDescription = "Limit",
                        Volume = 100,
                        StatusChangedOn = new DateTime(2018, 1, 1)
                    }
                };

            var result = projector.Project(docs);
            var firstResult = result.First();

            Assert.AreEqual(result.Count, 1);
            Assert.IsNotNull(firstResult.Limit);
            Assert.AreEqual(firstResult.Limit.Value.Value, 12);
            Assert.AreEqual(firstResult.Direction, OrderDirection.Buy);
            Assert.AreEqual(firstResult.OrderStatus, OrderStatus.Cancelled);
            Assert.AreEqual(firstResult.OrderType, OrderType.Limit);
            Assert.AreEqual(firstResult.Market.Id.Id, "LSE");
            Assert.AreEqual(firstResult.Market.Name, "LSE");
            Assert.AreEqual(firstResult.StatusChangedOn, new DateTime(2018, 1, 1));
            Assert.AreEqual(firstResult.Volume, 100);
            Assert.AreEqual(firstResult.Security.Id.Id, "STAN");
            Assert.AreEqual(firstResult.Security.Name, "Standard Chartered");
        }
    }
}
