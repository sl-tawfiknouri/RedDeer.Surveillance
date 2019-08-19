namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    using System;

    using Domain.Core.Financial.Assets.Interfaces;

    public interface ITimeSeriesTrendClassification
    {
        DateTime CommencementUtc { get; }

        IFinancialInstrument Instrument { get; }

        TimeSpan Length { get; }

        DateTime TerminationUtc { get; }

        TimeSegment TimeSegment { get; }

        TimeSeriesTrend Trend { get; }
    }
}