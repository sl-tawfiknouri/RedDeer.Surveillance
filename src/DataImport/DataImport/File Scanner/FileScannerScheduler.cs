using System;
using System.Timers;
using DataImport.File_Scanner.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.File_Scanner
{
    public class FileScannerScheduler : IFileScannerScheduler
    {
        private const int HeartbeatFrequency = 1000 * 60 * 60 * 24;

        private readonly IFileScanner _fileScanner;
        private readonly ILogger<FileScannerScheduler> _logger;

        private Timer _timer;

        public FileScannerScheduler(
            IFileScanner fileScanner,
            ILogger<FileScannerScheduler> logger)
        {
            _fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise()
        {
            _fileScanner.Scan().Wait();

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
                _logger.LogInformation($"FileScannerScheduler heart beat. Scanning data verifier and auto scheduler");
                _fileScanner.Scan().Wait();
                _logger.LogInformation($"FileScannerScheduler heart beat complete");
            }
            catch (Exception a)
            {
                _logger.LogError($"FileScannerScheduler encountered an exception {a.Message}");
            }
        }
    }
}
