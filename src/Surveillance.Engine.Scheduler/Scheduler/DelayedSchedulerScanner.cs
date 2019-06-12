using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

namespace Surveillance.Engine.Scheduler.Scheduler
{
    public class DelayedSchedulerScanner : IDelayedSchedulerScanner
    {
        private const int HeartbeatFrequency = 1000 * 60 * 15; // milliseconds (15 min)
        private readonly ILogger<DelayedSchedulerScanner> _logger;

        private Timer _timer;

        public DelayedSchedulerScanner(ILogger<DelayedSchedulerScanner> logger)
        {
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
        }

        private void Scan(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger?.LogInformation($"scanning delayed scheduler");
                
            }
            catch (Exception a)
            {
                _logger.LogError($"encountered an exception {a.Message} {a?.InnerException?.Message}", a);
            }
        }
    }
}
