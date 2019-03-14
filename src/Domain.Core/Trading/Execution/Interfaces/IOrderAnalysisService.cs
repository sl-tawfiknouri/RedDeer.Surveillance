using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Execution.Interfaces
{
    public interface IOrderAnalysisService
    {
        PriceSentiment ResolveSentiment(Order order);
        PriceSentiment ResolveSentiment(IReadOnlyCollection<Order> order);

        IOrderAnalysis AnalyseOrder(Order order);
        IReadOnlyCollection<IOrderAnalysis> AnalyseOrder(IReadOnlyCollection<Order> orders);

        IReadOnlyCollection<IOrderAnalysis> OpposingSentiment(IReadOnlyCollection<OrderAnalysis> orders, PriceSentiment sentiment);
    }
}