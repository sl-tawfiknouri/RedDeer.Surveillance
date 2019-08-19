namespace Domain.Core.Trading.Execution.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public interface IOrderAnalysisService
    {
        IOrderAnalysis AnalyseOrder(Order order);

        IReadOnlyCollection<IOrderAnalysis> AnalyseOrder(IReadOnlyCollection<Order> orders);

        IReadOnlyCollection<IOrderAnalysis> OpposingSentiment(
            IReadOnlyCollection<IOrderAnalysis> orders,
            PriceSentiment sentiment);

        PriceSentiment ResolveSentiment(Order order);

        PriceSentiment ResolveSentiment(IReadOnlyCollection<Order> order);
    }
}