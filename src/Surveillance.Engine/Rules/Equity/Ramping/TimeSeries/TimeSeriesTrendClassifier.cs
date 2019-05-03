using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Core.Markets.Timebars;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        public ITimeSeriesTrendClassification Classify(
            List<EquityInstrumentIntraDayTimeBar> timeBars,
            IFinancialInstrument financialInstrument, 
            DateTime commencement, 
            DateTime termination,
            TimeSegment timeSegment)
        {
            // ok so we have t1 -> t2
            // how do we classify the time series?
            // simple p@t1 >= p@t2

            // first things first lets get the raw data in here..

            return new TimeSeriesTrendClassification(
                financialInstrument,
                TimeSeriesTrend.Increasing,
                commencement,
                termination,
                timeSegment);
        }
    }
}
