using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Accord.Math.Distances;
using Domain.Trades.Orders;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Rules.WashTrade
{
    public class WashTradeClustering : IWashTradeClustering
    {
        public IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<TradeOrderFrame> frames)
        {
            if (frames == null
                || !frames.Any())
            {
                return new PositionClusterCentroid[0];
            }

            var filteredFrames = frames.Where(fra => fra.ExecutedPrice != null && fra.FulfilledVolume > 0).ToList();

            if (!filteredFrames.Any())
            {
                return new PositionClusterCentroid[0];
            }

            var clusterCount = OptimalClusterCount(frames);
            var kMeans = new KMeans(k: clusterCount) { Distance = new WeightedSquareEuclidean(new double[] { 2, 0.3 }) };

            var prices =
                filteredFrames
                    .Select(ff => new double[] { (double)ff.ExecutedPrice.Value.Value, ff.StatusChangedOn.Ticks})
                    .ToArray();

            var clusters = kMeans.Learn(prices);           
            int[] labels = clusters.Decide(prices);

            var groupedFrames = filteredFrames
                .Select((x, i) => new KeyValuePair<int, TradeOrderFrame>(i, x))
                .GroupBy(i => labels[i.Key]);

            var results = new List<PositionClusterCentroid>();
            
            foreach (var grp in groupedFrames)
            {
                var centroid = clusters.Centroids[labels[grp.First().Key]];
                var grpFrames = grp.Select(i => i.Value).ToList();
                var buys = new TradePosition(grpFrames.Where(i => i.Position == OrderPosition.Buy).ToList());
                var sells = new TradePosition(grpFrames.Where(i => i.Position == OrderPosition.Sell).ToList());

                if (buys.Get().Any() && sells.Get().Any())
                {
                    var positionCluster = new PositionClusterCentroid((decimal)centroid[0], buys, sells);
                    results.Add(positionCluster);
                }
            }

            return results;
        }

        private int OptimalClusterCount(IReadOnlyCollection<TradeOrderFrame> frames)
        {
            if (frames.Count < 10)
            {
                return 1;
            }

            if (frames.Count < 20)
            {
                return 3;
            }

            if (frames.Count < 100)
            {
                return 4;
            }

            // this is good for high numbers but leaves us with too many clusters at lower numbers

            return (int)Math.Ceiling(Math.Sqrt(frames.Count));
        }
    }
}
