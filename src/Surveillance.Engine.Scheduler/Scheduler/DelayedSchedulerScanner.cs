using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

namespace Surveillance.Engine.Scheduler.Scheduler
{
    public class DelayedSchedulerScanner : IDelayedSchedulerScanner
    {
        private const int HeartbeatFrequency = 1000 * 60 * 15; // milliseconds (15 min)
        private readonly IDelayedScheduler _delayedScheduler;
        private readonly ILogger<DelayedSchedulerScanner> _logger;

        private Timer _timer;

        public DelayedSchedulerScanner(
            IDelayedScheduler delayedScheduler,
            ILogger<DelayedSchedulerScanner> logger)
        {
            _delayedScheduler = delayedScheduler ?? throw new ArgumentNullException(nameof(delayedScheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation($"initiating delayed scheduler");

            Scan(null, null);

            _timer = new Timer(HeartbeatFrequency)
            {
                AutoReset = true,
                Interval = HeartbeatFrequency
            };

            _timer.Elapsed += Scan;
            _timer.Start();
        }

        public void Terminate()
        {
            _logger?.LogInformation($"terminating delayed scheduler");
            _timer?.Stop();
            _timer = null;
        }

        private void Scan(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger?.LogInformation($"scanning delayed scheduler");
                _delayedScheduler.ScheduleDueTasks();
                _logger?.LogInformation($"scanning delayed scheduler completed");
            }
            catch (Exception a)
            {
                _logger.LogError($"encountered an exception {a.Message} {a?.InnerException?.Message}", a);
            }
        }
    }
}
