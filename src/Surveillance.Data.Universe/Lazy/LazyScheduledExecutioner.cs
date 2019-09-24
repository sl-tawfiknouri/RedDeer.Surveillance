namespace Surveillance.Data.Universe.Lazy
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Data.Universe.Lazy.Interfaces;

    /// <summary>
    ///     Used to divide the scheduled execution into streamable chunks
    /// </summary>
    public class LazyScheduledExecutioner : ILazyScheduledExecutioner
    {
        public Stack<ScheduledExecution> Execute(ScheduledExecution schedule)
        {
            if (schedule == null) return new Stack<ScheduledExecution>();

            var span = schedule.AdjustedTimeSeriesTermination - schedule.AdjustedTimeSeriesInitiation;
            var response = new Stack<ScheduledExecution>();

            if (span.TotalDays < 8)
            {
                var splitSchedule = new ScheduledExecution
                                        {
                                            CorrelationId = schedule.CorrelationId,
                                            IsBackTest = schedule.IsBackTest,
                                            IsForceRerun = schedule.IsForceRerun,
                                            Rules = schedule.Rules,
                                            TimeSeriesInitiation = schedule.AdjustedTimeSeriesInitiation,
                                            TimeSeriesTermination = schedule.AdjustedTimeSeriesTermination
                                        };

                response.Push(splitSchedule);
                return response;
            }

            var initiation = schedule.AdjustedTimeSeriesInitiation;
            while (initiation < schedule.AdjustedTimeSeriesTermination)
            {
                var termination = initiation.AddDays(7);
                if (schedule.AdjustedTimeSeriesTermination < termination)
                    termination = schedule.AdjustedTimeSeriesTermination;

                var splitSchedule = new ScheduledExecution
                                        {
                                            CorrelationId = schedule.CorrelationId,
                                            IsBackTest = schedule.IsBackTest,
                                            IsForceRerun = schedule.IsForceRerun,
                                            Rules = schedule.Rules,
                                            TimeSeriesInitiation = initiation,
                                            TimeSeriesTermination = termination
                                        };

                response.Push(splitSchedule);
                initiation = termination;
            }

            return new Stack<ScheduledExecution>(response);
        }
    }
}