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
            TimeSpan span, 
            DateTime initiation)
        {
            return new ITimeSeriesTrendClassification[]
            {
                new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Increasing,
                    initiation,
                    initiation.Add(span),
                    TimeSegmentLength.ThirtyDay) 
            };
        }
    }
}
