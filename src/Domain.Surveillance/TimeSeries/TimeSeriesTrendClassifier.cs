using System;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Surveillance.TimeSeries.Interfaces;

namespace Domain.Surveillance.TimeSeries
{
    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        public TimeSeriesTrendClassification Classify(
            IFinancialInstrument financialInstrument,
            TimeSpan span,
            DateTime initiation)
        {
            return TimeSeriesTrendClassification.Chaotic;
        }
    }
}
