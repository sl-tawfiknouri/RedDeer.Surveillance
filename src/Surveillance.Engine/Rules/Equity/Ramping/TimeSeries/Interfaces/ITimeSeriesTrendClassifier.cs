namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Core.Markets.Timebars;

    public interface ITimeSeriesTrendClassifier
    {
        ITimeSeriesTrendClassification Classify(
            List<EquityInstrumentIntraDayTimeBar> timeBars,
            IFinancialInstrument financialInstrument,
            DateTime commencement,
            DateTime termination,
            TimeSegment timeSegment);
    }
}