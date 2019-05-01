using System;
using System.Collections.Generic;
using Domain.Core.Financial.Assets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    public interface ITimeSeriesTrendClassifier
    {
        IReadOnlyCollection<ITimeSeriesTrendClassification> Classify(
            IFinancialInstrument financialInstrument,
            TimeSpan span,
            DateTime initiation);
    }
}