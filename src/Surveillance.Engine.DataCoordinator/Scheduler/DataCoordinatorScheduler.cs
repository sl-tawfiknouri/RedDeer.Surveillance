namespace Surveillance.Engine.DataCoordinator.Scheduler
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    /// <summary>
    /// Timer to check for data health issue and run auto schedules
    /// </summary>
    public class DataCoordinatorScheduler : IDataCoordinatorScheduler
    {
        /// <summary>
        /// The heartbeat frequency milliseconds (20 min).
        /// </summary>
        private const int HeartbeatFrequency = 1000 * 60 * 20;

        /// <summary>
        /// The auto scheduler.
        /// </summary>
        private readonly IAutoSchedule autoScheduler;

        /// <summary>
        /// The data verifier.
        /// </summary>
        private readonly IDataVerifier dataVerifier;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DataCoordinatorScheduler> logger;

        /// <summary>
        /// The timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCoordinatorScheduler"/> class.
        /// </summary>
        /// <param name="dataVerifier">
        /// The data verifier.
        /// </param>
        /// <param name="autoScheduler">
        /// The auto scheduler.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DataCoordinatorScheduler(
            IDataVerifier dataVerifier,
            IAutoSchedule autoScheduler,
            ILogger<DataCoordinatorScheduler> logger)
        {
            this.dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            this.autoScheduler = autoScheduler ?? throw new ArgumentNullException(nameof(autoScheduler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialise()
        {
            this.TimerOnElapsed(null, null);

            this.timer = 
                new Timer(HeartbeatFrequency)
                    {
                        AutoReset = true,
                        Interval = HeartbeatFrequency
                    };

            this.timer.Elapsed += (_, __) => this.TimerOnElapsed(_, __);
            this.timer.Start();
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.timer?.Stop();
        }

        /// <summary>
        /// The timer on elapsed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.logger.LogInformation("heart beat. Scanning data verifier and auto scheduler");
                await this.dataVerifier.Scan().ConfigureAwait(false);
                await this.autoScheduler.Scan().ConfigureAwait(false);
                this.logger.LogInformation("heart beat complete");
            }
            catch (Exception a)
            {
                this.logger.LogError($"encountered an exception {a.Message}");
            }
        }
    }
}