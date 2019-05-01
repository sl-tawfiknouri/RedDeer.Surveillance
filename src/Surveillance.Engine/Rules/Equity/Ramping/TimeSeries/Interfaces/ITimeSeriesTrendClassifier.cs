using System;
using Domain.Core.Financial.Assets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    public interface ITimeSeriesTrendClassifier
    {
        TimeSeriesTrendClassification Classify(
            IFinancialInstrument financialInstrument,
            TimeSpan span,
            DateTime initiation);
    }
}