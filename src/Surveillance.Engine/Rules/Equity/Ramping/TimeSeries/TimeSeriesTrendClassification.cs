namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    using System;

    using Domain.Core.Financial.Assets.Interfaces;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

    public class TimeSeriesTrendClassification : ITimeSeriesTrendClassification
    {
        public TimeSeriesTrendClassification(
            IFinancialInstrument instrument,
            TimeSeriesTrend trend,
            DateTime commencementUtc,
            DateTime terminationUtc,
            TimeSegment timeSegment)
        {
            this.Instrument = instrument ?? throw new ArgumentNullException(nameof(instrument));
            this.Trend = trend;
            this.CommencementUtc = commencementUtc;
            this.TerminationUtc = terminationUtc;
            this.TimeSegment = timeSegment;
        }

        public DateTime CommencementUtc { get; }

        public IFinancialInstrument Instrument { get; }

        public TimeSpan Length => this.TerminationUtc - this.CommencementUtc;

        public DateTime TerminationUtc { get; }

        public TimeSegment TimeSegment { get; }

        public TimeSeriesTrend Trend { get; }
    }
}