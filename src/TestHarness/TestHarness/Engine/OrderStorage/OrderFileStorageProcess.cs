// ReSharper disable InconsistentlySynchronizedField

namespace TestHarness.Engine.OrderStorage
{
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

    public class OrderFileStorageProcess : IOrderFileStorageProcess
    {
        private readonly IConsole _console;

        private readonly string _fileStamp;

        private readonly List<Order> _frames;

        private readonly object _lock = new object();

        private readonly ILogger _logger;

        private readonly IOrderFileToOrderSerialiser _orderSerialiser;

        private readonly string _storagePath;

        private readonly Timer _timer;

        private volatile bool _timerInitiated;

        public OrderFileStorageProcess(
            string storagePath,
            IConsole console,
            IOrderFileToOrderSerialiser orderSerialiser,
            ILogger logger)
        {
            this._storagePath = storagePath ?? "GeneratedOrderFiles";
            this._console = console ?? throw new ArgumentNullException(nameof(console));
            this._orderSerialiser = orderSerialiser ?? throw new ArgumentNullException(nameof(orderSerialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._frames = new List<Order>();
            this._timerInitiated = false;
            this._fileStamp = $"TradeFile-{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.csv";

            this._timer = new Timer { AutoReset = false, Interval = 30 * 1000 };
            this._timer.Elapsed += this.OnElapse;

            this.Initialise();
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            this._logger.LogError(error, "Exception");
        }

        public void OnNext(Order value)
        {
            lock (this._lock)
            {
                if (!this._timerInitiated)
                {
                    this._timer.Start();
                    this._timerInitiated = true;
                    this._console.WriteToUserFeedbackLine("Queued up trade file update in 30 seconds...");
                }

                this._frames.Add(value);
            }
        }

        private void Initialise()
        {
            try
            {
                if (!Directory.Exists(this._storagePath)) Directory.CreateDirectory(this._storagePath);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Exception");
            }
        }

        private void OnElapse(object sender, ElapsedEventArgs e)
        {
            lock (this._lock)
            {
                this._timer.Stop();

                if (!this._frames.Any()) return;

                try
                {
                    var filePath = Path.Combine(this._storagePath, this._fileStamp);

                    var csvFrames = this._frames.Select(this._orderSerialiser.Map).ToList().SelectMany(x => x).ToList();

                    using (var writer = File.CreateText(filePath))
                    {
                        var csv = new CsvWriter(writer);
                        csv.Configuration.HasHeaderRecord = true;
                        csv.WriteRecords(csvFrames);
                    }

                    this._timerInitiated = false;
                    this._console.WriteToUserFeedbackLine("Trade file updated.");
                }
                catch (Exception a)
                {
                    this._logger.LogError(a, "Exception");
                    this._timer.Start();
                }
            }
        }
    }
}