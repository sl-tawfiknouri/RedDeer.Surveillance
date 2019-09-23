namespace Surveillance.Engine.Scheduler.Scheduler
{
    using System;
    using System.Timers;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    public class DelayedSchedulerScanner : IDelayedSchedulerScanner
    {
        private const int HeartbeatFrequency = 1000 * 60 * 15; // milliseconds (15 min)

        private readonly IDelayedScheduler _delayedScheduler;

        private readonly ILogger<DelayedSchedulerScanner> _logger;

        private Timer _timer;

        public DelayedSchedulerScanner(IDelayedScheduler delayedScheduler, ILogger<DelayedSchedulerScanner> logger)
        {
            this._delayedScheduler = delayedScheduler ?? throw new ArgumentNullException(nameof(delayedScheduler));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            this._logger?.LogInformation("initiating delayed scheduler");

            this.Scan(null, null);

            this._timer = new Timer(HeartbeatFrequency) { AutoReset = true, Interval = HeartbeatFrequency };

            this._timer.Elapsed += this.Scan;
            this._timer.Start();
        }

        public void Terminate()
        {
            this._logger?.LogInformation("terminating delayed scheduler");
            this._timer?.Stop();
            this._timer = null;
        }

        private void Scan(object sender, ElapsedEventArgs e)
        {
            try
            {
                this._logger?.LogInformation("scanning delayed scheduler");
                this._delayedScheduler.ScheduleDueTasks().Wait();
                this._logger?.LogInformation("scanning delayed scheduler completed");
            }
            catch (Exception a)
            {
                this._logger.LogError($"encountered an exception {a.Message} {a?.InnerException?.Message}", a);
            }
        }
    }
}