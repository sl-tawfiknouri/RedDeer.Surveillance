using System;
using Domain.Core.Financial.Assets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        public TimeSeriesTrendClassification Classify(
            IFinancialInstrument financialInstrument,
            TimeSpan span,
            DateTime initiation)
        {
            return TimeSeriesTrendClassification.Increasing;
        }
    }
}
