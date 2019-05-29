using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using CsvHelper;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders.Interfaces;
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

        private readonly List<Order> _frames;
        private readonly Timer _timer;
        private readonly string _fileStamp;

        private readonly IConsole _console;
        private readonly IOrderFileToOrderSerialiser _orderSerialiser;

        public OrderFileStorageProcess(
            string storagePath,
            IConsole console,
            IOrderFileToOrderSerialiser orderSerialiser,
            ILogger logger)
        {
            _storagePath = storagePath ?? "GeneratedOrderFiles";
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _orderSerialiser = orderSerialiser ?? throw new ArgumentNullException(nameof(orderSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _frames = new List<Order>();
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
                _logger.LogError(e.Message);
            }
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        {
            _logger.LogError(error.Message);
        }

        public void OnNext(Order value)
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

                    var csvFrames = _frames.Select(_orderSerialiser.Map).ToList().SelectMany(x => x).ToList();

                    using (var writer = File.CreateText(filePath))
                    {
                        var csv = new CsvWriter(writer);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.WriteRecords(csvFrames);
                    }

                    _timerInitiated = false;
                    _console.WriteToUserFeedbackLine($"Trade file updated.");
                }
                catch (Exception a)
                {
                    _logger.LogError(a.Message);
                    _timer.Start();
                }
            }
        }
    }
}
