using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Accord.Math.Distances;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    public class ClusteringService : IClusteringService
    {
        public IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames)
        {
            if (frames == null
                || !frames.Any())
            {
                return new PositionClusterCentroid[0];
            }

            var filteredFrames = frames.Where(fra => fra.OrderAverageFillPrice != null && fra.OrderFilledVolume > 0).ToList();

            if (!filteredFrames.Any())
            {
                return new PositionClusterCentroid[0];
            }

            var clusterCount = OptimalClusterCount(frames);
            var kMeans = new KMeans(k: clusterCount) { Distance = new WeightedSquareEuclidean(new double[] { 2, 0.3 }) };

            var prices =
                filteredFrames
                    .Select(ff => new double[] { (double)ff.OrderAverageFillPrice.GetValueOrDefault().Value, ff.MostRecentDateEvent().Ticks})
                    .ToArray();

            var clusters = kMeans.Learn(prices);           
            int[] labels = clusters.Decide(prices);

            var groupedFrames = filteredFrames
                .Select((x, i) => new KeyValuePair<int, Order>(i, x))
                .GroupBy(i => labels[i.Key]);

            var results = new List<PositionClusterCentroid>();
            
            foreach (var grp in groupedFrames)
            {
                var centroid = clusters.Centroids[labels[grp.First().Key]];
                var grpFrames = grp.Select(i => i.Value).ToList();
                var buys = new TradePosition(grpFrames.Where(i => i.OrderDirection == OrderDirections.BUY || i.OrderDirection == OrderDirections.COVER).ToList());
                var sells = new TradePosition(grpFrames.Where(i => i.OrderDirection == OrderDirections.SELL || i.OrderDirection == OrderDirections.SHORT).ToList());

                if (buys.Get().Any() && sells.Get().Any())
                {
                    var positionCluster = new PositionClusterCentroid((decimal)centroid[0], buys, sells);
                    results.Add(positionCluster);
                }
            }

            return results;
        }

        private int OptimalClusterCount(IReadOnlyCollection<Order> frames)
        {
            if (frames.Count < 3)
            {
                return 1;
            }

            if (frames.Count < 6)
            {
                return 2;
            }

            if (frames.Count < 20)
            {
                return 3;
            }

            if (frames.Count < 100)
            {
                return 4;
            }

            return (int)Math.Ceiling(Math.Sqrt(frames.Count) - 5);
        }
    }
}
