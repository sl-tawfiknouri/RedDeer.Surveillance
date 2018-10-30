using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using CsvHelper;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Display.Interfaces;
using TestHarness.Engine.OrderStorage.Interfaces;
// ReSharper disable InconsistentlySynchronizedField

namespace TestHarness.Engine.OrderStorage
{
    public class OrderFileStorageProcess : IOrderFileStorageProcess
    {
        private readonly object _lock = new object();
        private readonly string _storagePath;
        private readonly ILogger _logger;

        private volatile bool _timerInitiated;

        private readonly List<TradeOrderFrame> _frames;
        private readonly Timer _timer;
        private readonly string _fileStamp;

        private readonly IConsole _console;

        public OrderFileStorageProcess(
            string storagePath,
            IConsole console,
            ILogger logger)
        {
            _storagePath = storagePath ?? "GeneratedOrderFiles";
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _frames = new List<TradeOrderFrame>();
            _timerInitiated = false;
            _fileStamp = $"TradeFile-{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.csv";

            _timer = new Timer {AutoReset = false, Interval = 30 * 1000};
            _timer.Elapsed += OnElapse;

            Initialise();
        }

        private void Initialise()
        {
            try
            {
                if (!Directory.Exists(_storagePath))
                {
                    Directory.CreateDirectory(_storagePath);
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e);
            }
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            _logger.Log(LogLevel.Error, error);
        }

        public void OnNext(TradeOrderFrame value)
        {
            lock (_lock)
            {
                if (!_timerInitiated)
                {
                    _timer.Start();
                    _timerInitiated = true;
                    _console.WriteToUserFeedbackLine($"Queued up trade file update in 30 seconds...");
                }

                _frames.Add(value);
            }
        }

        private void OnElapse(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                _timer.Stop();

                if (!_frames.Any())
                {
                    return;
                }

                try
                {
                    var filePath = Path.Combine(_storagePath, _fileStamp);

                    using (var writer = File.CreateText(filePath))
                    {
                        var csv = new CsvWriter(writer);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.WriteRecords(_frames);
                    }

                    _timerInitiated = false;
                    _console.WriteToUserFeedbackLine($"Trade file updated.");
                }
                catch (Exception a)
                {
                    _logger.Log(LogLevel.Error, a);
                    _timer.Start();
                }
            }
        }
    }
}
