using System.Collections.Generic;
using Domain.Surveillance.Scheduling;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Lazy
{
    /// <summary>
    /// Used to divide the scheduled execution into streamable chunks
    /// </summary>
    public class LazyScheduledExecutioner : ILazyScheduledExecutioner
    {
        public Stack<ScheduledExecution> Execute(ScheduledExecution schedule)
        {
            if (schedule == null)
            {
                return new Stack<ScheduledExecution>();
            }

            var span = schedule.TimeSeriesTermination - schedule.TimeSeriesInitiation;
            var response = new Stack<ScheduledExecution>();

            if (span.TotalDays < 5)
            {
                response.Push(schedule);
                return response;
            }

            var initiation = schedule.TimeSeriesInitiation;
            while (initiation < schedule.TimeSeriesTermination)
            {
                var termination = initiation.AddDays(4);
                if (schedule.TimeSeriesTermination < termination)
                {
                    termination = schedule.TimeSeriesTermination;
                }

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

            return response;
        }
    }
}
