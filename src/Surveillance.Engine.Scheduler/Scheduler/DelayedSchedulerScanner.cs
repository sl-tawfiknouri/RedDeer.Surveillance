namespace Surveillance.Engine.Scheduler.Scheduler
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Scheduler.Scheduler.Interfaces;

    /// <summary>
    /// The delayed scheduler scanner.
    /// </summary>
    public class DelayedSchedulerScanner : IDelayedSchedulerScanner
    {
        /// <summary>
        /// The heartbeat frequency.
        /// </summary>
        private const int HeartbeatFrequency = 1000 * 60 * 15; // milliseconds (15 min)

        /// <summary>
        /// The delayed scheduler.
        /// </summary>
        private readonly IDelayedScheduler delayedScheduler;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DelayedSchedulerScanner> logger;

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedSchedulerScanner"/> class.
        /// </summary>
        /// <param name="delayedScheduler">
        /// The delayed scheduler.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DelayedSchedulerScanner(IDelayedScheduler delayedScheduler, ILogger<DelayedSchedulerScanner> logger)
        {
            this.delayedScheduler = delayedScheduler ?? throw new ArgumentNullException(nameof(delayedScheduler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger?.LogInformation("initiating delayed scheduler");

            this.Scan(null, null)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            this.timer = new Timer(HeartbeatFrequency) { AutoReset = true, Interval = HeartbeatFrequency };

            this.timer.Elapsed += async (sender, elapsedEventArgs) => await this.Scan(sender, elapsedEventArgs);
            this.timer.Start();
        }

        public void Terminate()
        {
            this.logger?.LogInformation("terminating delayed scheduler");
            this.timer?.Stop();
            this.timer = null;
        }

        private async Task Scan(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.logger?.LogInformation("scanning delayed scheduler");
                await this.delayedScheduler.ScheduleDueTasksAsync();
                this.logger?.LogInformation("scanning delayed scheduler completed");
            }
            catch (Exception a)
            {
                this.logger.LogError(a, $"encountered an exception");
            }
        }
    }
}