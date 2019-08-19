namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

    public interface IOrderPriceImpactClassifier
    {
        IPriceImpactSummary ClassifyByTradeCount(IReadOnlyCollection<Order> orders, TimeSegment segment);

        IPriceImpactSummary ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders, TimeSegment segment);
    }
}