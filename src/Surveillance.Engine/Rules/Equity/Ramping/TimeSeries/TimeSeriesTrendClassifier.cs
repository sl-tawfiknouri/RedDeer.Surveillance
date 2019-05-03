using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        IReadOnlyCollection<ITimeSeriesTrendClassification> ITimeSeriesTrendClassifier.Classify(
            IFinancialInstrument financialInstrument, 
            DateTime commencement, 
            DateTime termination)
        {
            return new ITimeSeriesTrendClassification[]
            {
                new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Increasing,
                    commencement,
                    termination,
                    TimeSegment.ThirtyDay) 
            };
        }
    }
}
