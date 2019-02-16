using System;
using System.Timers;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Scheduler
{
    /// <summary>
    /// Timer to check for data health issue and run auto schedules
    /// </summary>
    public class DataCoordinatorScheduler : IDataCoordinatorScheduler
    {
        private const int HeartbeatFrequency = 1000 * 60 * 5; // milliseconds (5 min)

        private readonly IDataVerifier _dataVerifier;
        private readonly IAutoSchedule _autoScheduler;
        private readonly ILogger<DataCoordinatorScheduler> _logger;

        private Timer _timer;

        public DataCoordinatorScheduler(
            IDataVerifier dataVerifier,
            IAutoSchedule autoScheduler,
            ILogger<DataCoordinatorScheduler> logger)
        {
            _dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            _autoScheduler = autoScheduler ?? throw new ArgumentNullException(nameof(autoScheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise()
        {
            TimerOnElapsed(null, null);

            _timer = new Timer(HeartbeatFrequency)
            {
                AutoReset = true,
                Interval = HeartbeatFrequency
            };

            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void Terminate()
        {
            _timer?.Stop();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _logger.LogInformation($"DataCoordinatorScheduler heart beat. Scanning data verifier and auto scheduler");
                _dataVerifier.Scan().Wait();
                _autoScheduler.Scan().Wait();
                _logger.LogInformation($"DataCoordinatorScheduler heart beat complete");
            }
            catch (Exception a)
            {
                _logger.LogError($"DataCoordinatorScheduler encountered an exception {a.Message}");
            }
        }
    }
}
