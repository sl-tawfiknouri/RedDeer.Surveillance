using System;
using Domain.Core.Financial.Assets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        ITimeSeriesTrendClassification ITimeSeriesTrendClassifier.Classify(
            IFinancialInstrument financialInstrument, 
            DateTime commencement, 
            DateTime termination)
        {
            return new TimeSeriesTrendClassification(
                financialInstrument,
                TimeSeriesTrend.Increasing,
                commencement,
                termination,
                TimeSegment.ThirtyDay);
        }
    }
}
