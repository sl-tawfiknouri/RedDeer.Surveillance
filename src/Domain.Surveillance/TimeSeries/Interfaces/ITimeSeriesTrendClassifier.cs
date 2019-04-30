using System;
using Domain.Core.Financial.Assets.Interfaces;

namespace Domain.Surveillance.TimeSeries.Interfaces
{
    public interface ITimeSeriesTrendClassifier
    {
        TimeSeriesTrendClassification Classify(
            IFinancialInstrument financialInstrument,
            TimeSpan span,
            DateTime initiation);
    }
}