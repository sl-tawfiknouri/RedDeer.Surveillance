namespace Surveillance.Engine.DataCoordinator.Scheduler
{
    using System;
    using System.Timers;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    /// <summary>
    ///     Timer to check for data health issue and run auto schedules
    /// </summary>
    public class DataCoordinatorScheduler : IDataCoordinatorScheduler
    {
        private const int HeartbeatFrequency = 1000 * 60 * 20; // milliseconds (20 min)

        private readonly IAutoSchedule _autoScheduler;

        private readonly IDataVerifier _dataVerifier;

        private readonly ILogger<DataCoordinatorScheduler> _logger;

        private Timer _timer;

        public DataCoordinatorScheduler(
            IDataVerifier dataVerifier,
            IAutoSchedule autoScheduler,
            ILogger<DataCoordinatorScheduler> logger)
        {
            this._dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            this._autoScheduler = autoScheduler ?? throw new ArgumentNullException(nameof(autoScheduler));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise()
        {
            this.TimerOnElapsed(null, null);

            this._timer = new Timer(HeartbeatFrequency) { AutoReset = true, Interval = HeartbeatFrequency };

            this._timer.Elapsed += this.TimerOnElapsed;
            this._timer.Start();
        }

        public void Terminate()
        {
            this._timer?.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this._logger.LogInformation("heart beat. Scanning data verifier and auto scheduler");
                this._dataVerifier.Scan().Wait();
                this._autoScheduler.Scan().Wait();
                this._logger.LogInformation("heart beat complete");
            }
            catch (Exception a)
            {
                this._logger.LogError($"encountered an exception {a.Message}");
            }
        }
    }
}