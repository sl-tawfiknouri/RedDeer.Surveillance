using System;
using Domain.Core.Financial.Assets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    public interface ITimeSeriesTrendClassification
    {
        DateTime CommencementUtc { get; }
        IFinancialInstrument Instrument { get; }
        TimeSpan Length { get; }
        DateTime TerminationUtc { get; }
        TimeSeriesTrend Trend { get; }
        TimeSegment TimeSegment { get; }
    }
}