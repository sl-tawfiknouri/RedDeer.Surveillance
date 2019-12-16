namespace DataImport.File_Scanner
{
    using System;
    using System.Timers;

    using DataImport.File_Scanner.Interfaces;

    using Microsoft.Extensions.Logging;

    public class FileScannerScheduler : IFileScannerScheduler
    {
        private const int HeartbeatFrequency = 1000 * 60 * 60 * 24;

        private readonly IFileScanner _fileScanner;

        private readonly ILogger<FileScannerScheduler> _logger;

        private Timer _timer;

        public FileScannerScheduler(IFileScanner fileScanner, ILogger<FileScannerScheduler> logger)
        {
            this._fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise()
        {
            this._fileScanner.Scan().Wait();

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
                this._logger.LogInformation(
                    "FileScannerScheduler heart beat. Scanning data verifier and auto scheduler");
                this._fileScanner.Scan().Wait();
                this._logger.LogInformation("FileScannerScheduler heart beat complete");
            }
            catch (Exception a)
            {
                this._logger.LogError(a, $"FileScannerScheduler encountered an exception");
            }
        }
    }
}