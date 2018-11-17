using Domain.Trades.Orders;
using NUnit.Framework;
using Surveillance.Rules.WashTrade;
using Surveillance.Tests.Helpers;

namespace Surveillance.Tests.Rules.WashTrades
{
    [TestFixture]
    public class WashTradeClusteringTests
    {
        [Test]
        public void Clustering_ReturnsEmpty_ForNullArgs()
        {
            var clustering = new WashTradeClustering();

            var cluster = clustering.Cluster(null);

            Assert.IsNotNull(cluster);
            Assert.AreEqual(cluster.Count, 0);
        }

        [Test]
        [Explicit]
        public void Clustering_TwoFrames()
        {
            var clustering = new WashTradeClustering();
            var frame1 = (new TradeOrderFrame()).Random(10);
            var frame2 = (new TradeOrderFrame()).Random(11);
            var frame3 = (new TradeOrderFrame()).Random(12);
            var frame4 = (new TradeOrderFrame()).Random(13);
            var frame5 = (new TradeOrderFrame()).Random(18);
            var frame6 = (new TradeOrderFrame()).Random(19);
            var frame7 = (new TradeOrderFrame()).Random(20);
            var frame8 = (new TradeOrderFrame()).Random(21);
            var frame9 = (new TradeOrderFrame()).Random(22);

            frame1.Position = OrderPosition.Sell;
            frame3.Position = OrderPosition.Sell;
            frame5.Position = OrderPosition.Sell;
            frame7.Position = OrderPosition.Sell;
            frame9.Position = OrderPosition.Sell;

            var frames = new[] {frame1, frame2, frame3, frame4, frame5, frame6, frame7, frame8, frame9 };

            var results = clustering.Cluster(frames);

            var hm = results;
        }
    }
}