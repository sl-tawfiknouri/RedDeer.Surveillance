namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

    public interface IRampingAnalyser
    {
        IRampingStrategySummaryPanel Analyse(
            IReadOnlyCollection<Order> orderSegment,
            List<EquityInstrumentIntraDayTimeBar> timeBars);
    }
}