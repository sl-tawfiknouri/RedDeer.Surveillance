namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets.Interfaces;
    using Domain.Core.Markets.Timebars;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

    public class TimeSeriesTrendClassifier : ITimeSeriesTrendClassifier
    {
        private readonly ILogger<TimeSeriesTrendClassifier> _logger;

        public TimeSeriesTrendClassifier(ILogger<TimeSeriesTrendClassifier> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITimeSeriesTrendClassification Classify(
            List<EquityInstrumentIntraDayTimeBar> timeBars,
            IFinancialInstrument financialInstrument,
            DateTime commencement,
            DateTime termination,
            TimeSegment timeSegment)
        {
            if (timeBars == null || !timeBars.Any())
            {
                this._logger?.LogWarning("Received an empty or null time bars collection");

                return new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Unclassified,
                    commencement,
                    termination,
                    timeSegment);
            }

            var orderedTimeBars = timeBars.OrderBy(_ => _.TimeStamp).ToList();

            // basic implementation for initial work - we will make this more sophisticated in response to tests
            var initial = orderedTimeBars.FirstOrDefault();
            var final = orderedTimeBars.LastOrDefault();

            if (initial == null || final == null)
                return new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Unclassified,
                    commencement,
                    termination,
                    timeSegment);

            // TODO take a volatiltiy measure and return chaotic if volatiltiy is high
            if (final.SpreadTimeBar.Price.Value > initial.SpreadTimeBar.Price.Value)
                return new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Increasing,
                    commencement,
                    termination,
                    timeSegment);

            if (final.SpreadTimeBar.Price.Value < initial.SpreadTimeBar.Price.Value)
                return new TimeSeriesTrendClassification(
                    financialInstrument,
                    TimeSeriesTrend.Decreasing,
                    commencement,
                    termination,
                    timeSegment);

            return new TimeSeriesTrendClassification(
                financialInstrument,
                TimeSeriesTrend.Unclassified,
                commencement,
                termination,
                timeSegment);
        }
    }
}