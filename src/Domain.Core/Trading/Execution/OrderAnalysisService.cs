using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Execution.Interfaces;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Execution
{
    public class OrderAnalysisService : IOrderAnalysisService
    {
        private readonly IReadOnlyCollection<OrderDirections> _positiveSentiments = 
            new List<OrderDirections>
            {
                OrderDirections.BUY,
                OrderDirections.COVER
            };

        private readonly IReadOnlyCollection<OrderDirections> _negativeSentiments =
            new List<OrderDirections>
            {
                OrderDirections.SELL,
                OrderDirections.SHORT
            };

        public PriceSentiment ResolveSentiment(IReadOnlyCollection<Order> order)
        {
            if (order == null
                || !order.Any())
            {
                return PriceSentiment.Neutral;
            }

            if (order.All(i => _positiveSentiments.Contains(i.OrderDirection)))
            {
                return PriceSentiment.Positive;
            }

            if (order.All(i => _negativeSentiments.Contains(i.OrderDirection)))
            {
                return PriceSentiment.Negative;
            }

            return PriceSentiment.Mixed;
        }

        public PriceSentiment ResolveSentiment(Order order)
        {
            if (order == null)
            {
                return PriceSentiment.Neutral;
            }

            if (_positiveSentiments.Contains(order.OrderDirection))
            {
                return PriceSentiment.Positive;
            }

            if (_negativeSentiments.Contains(order.OrderDirection))
            {
                return PriceSentiment.Negative;
            }

            return PriceSentiment.Neutral;
        }

        public IReadOnlyCollection<IOrderAnalysis> AnalyseOrder(IReadOnlyCollection<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return new IOrderAnalysis[0];
            }

            return orders.Select(AnalyseOrder).ToList();
        }

        public IOrderAnalysis AnalyseOrder(Order order)
        {
            if (order == null)
            {
                return null;
            }

            return new OrderAnalysis(order, ResolveSentiment(order));
        }

        public IReadOnlyCollection<IOrderAnalysis> OpposingSentiment(IReadOnlyCollection<OrderAnalysis> orders, PriceSentiment sentiment)
        {
            if (sentiment == PriceSentiment.Neutral
                || sentiment == PriceSentiment.Mixed)
            {
                return new IOrderAnalysis[0];
            }

            if (sentiment == PriceSentiment.Positive)
            {
                return orders.Where(i => _negativeSentiments.Contains(i.Order.OrderDirection)).ToList();
            }

            if (sentiment == PriceSentiment.Negative)
            {
                return orders.Where(i => _positiveSentiments.Contains(i.Order.OrderDirection)).ToList();
            }

            throw new ArgumentOutOfRangeException(nameof(sentiment));
        }
    }
}