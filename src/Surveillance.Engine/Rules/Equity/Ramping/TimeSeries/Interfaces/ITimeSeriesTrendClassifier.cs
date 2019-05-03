using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Core.Markets.Timebars;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
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