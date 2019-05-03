using System;
using Domain.Core.Financial.Assets.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces
{
    public interface ITimeSeriesTrendClassifier
    {
        ITimeSeriesTrendClassification Classify(
            IFinancialInstrument financialInstrument,
            DateTime commencement,
            DateTime termination);
    }
}