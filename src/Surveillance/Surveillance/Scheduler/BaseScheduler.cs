using System;
using DomainV2.Scheduling;
using Microsoft.Extensions.Logging;

namespace Surveillance.Scheduler
{
    public abstract class BaseScheduler
    {
        private readonly ILogger _logger;

        protected BaseScheduler(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected bool ValidateScheduleRule(ScheduledExecution execution)
        {
            if (execution == null)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a null scheduled execution. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation.DateTime.Year < 2015)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series initiation before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesTermination.DateTime.Year < 2015)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series termination before 2015. Returning.");
                return false;
            }

            if (execution.TimeSeriesInitiation > execution.TimeSeriesTermination)
            {
                _logger?.LogError($"ReddeerRuleScheduler had a time series initiation that exceeded the time series termination.");
                return false;
            }

            return true;
        }
    }
}
