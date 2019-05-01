using System;
using Domain.Core.Financial.Assets.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public class TimeSeriesTrendClassification : ITimeSeriesTrendClassification
    {
        public TimeSeriesTrendClassification(
            IFinancialInstrument instrument,
            TimeSeriesTrend trend, 
            DateTime commencementUtc,
            DateTime terminationUtc)
        {
            Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            Trend = trend;
            CommencementUtc = commencementUtc;
            TerminationUtc = terminationUtc;
        }

        public IFinancialInstrument Instrument { get; }

        public TimeSeriesTrend Trend { get; }

        public DateTime CommencementUtc { get; }
        public DateTime TerminationUtc { get; }

        public TimeSpan Length => TerminationUtc - CommencementUtc;
    }
}
