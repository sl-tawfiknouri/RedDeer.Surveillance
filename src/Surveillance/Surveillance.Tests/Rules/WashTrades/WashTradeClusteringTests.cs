using DomainV2.Financial;
using DomainV2.Trading;
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
            var frame1 = (new Order()).Random(10);
            var frame2 = (new Order()).Random(11);
            var frame3 = (new Order()).Random(12);
            var frame4 = (new Order()).Random(13);
            var frame5 = (new Order()).Random(18);
            var frame6 = (new Order()).Random(19);
            var frame7 = (new Order()).Random(20);
            var frame8 = (new Order()).Random(21);
            var frame9 = (new Order()).Random(22);

            frame1.OrderPosition = OrderPositions.SELL;
            frame3.OrderPosition = OrderPositions.SELL;
            frame5.OrderPosition = OrderPositions.SELL;
            frame7.OrderPosition = OrderPositions.SELL;
            frame9.OrderPosition = OrderPositions.SELL;

            var frames = new[] {frame1, frame2, frame3, frame4, frame5, frame6, frame7, frame8, frame9 };

            var results = clustering.Cluster(frames);

            var hm = results;
        }
    }
}