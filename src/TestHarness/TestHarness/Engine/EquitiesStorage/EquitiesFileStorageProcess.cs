namespace TestHarness.Engine.EquitiesStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CsvHelper;

    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Security;
    using SharedKernel.Files.Security.Interfaces;

    using TestHarness.Engine.EquitiesStorage.Interfaces;

    public class EquitiesFileStorageProcess : IEquityDataStorage
    {
        private readonly ILogger _logger;

        private readonly string _path;

        private readonly IDtoToSecurityCsvMapper _securityMapper;

        private readonly object _stateTransition = new object();

        private IStockExchangeStream _stockStream;

        private IDisposable _unsubscriber;

        public EquitiesFileStorageProcess(string path, ILogger logger, IDtoToSecurityCsvMapper securityMapper)
        {
            this._path = path ?? string.Empty;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._securityMapper = securityMapper ?? throw new ArgumentNullException(nameof(securityMapper));
        }

        public void Initiate(IStockExchangeStream stockStream)
        {
            lock (this._stateTransition)
            {
                if (stockStream == null)
                {
                    this._logger.Log(
                        LogLevel.Error,
                        "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                this._stockStream = stockStream;
                this._unsubscriber = stockStream.Subscribe(this);
            }

            if (string.IsNullOrWhiteSpace(this._path))
            {
                this._logger.LogError(
                    "Equities File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!Directory.Exists(this._path)) Directory.CreateDirectory(this._path);
        }

        public void OnCompleted()
        {
            this._logger.LogInformation(
                "Order data generator received completed message from stock stream. Terminating equities file storage");
            this.Terminate();
        }

        public void OnError(Exception error)
        {
            this._logger.LogError(error, "Exception");
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            var csvRecords = value?.Securities?.Select(this._securityMapper.Map).Where(w => w != null).ToList()
                             ?? new List<FinancialInstrumentTimeBarCsv>();

            if (!csvRecords.Any()) return;

            var fileName = $"{value.Exchange.Id}-{value.Epoch.ToString("yyyyMMddHHmmssffff")}.csv";
            var filePath = Path.Combine(this._path, fileName);

            using (var writer = File.CreateText(filePath))
            {
                var csv = new CsvWriter(writer);
                csv.Configuration.HasHeaderRecord = true;
                csv.WriteRecords(csvRecords);
            }
        }

        /// <summary>
        ///     Avoid calling this from inside another state transition
        /// </summary>
        public void Terminate()
        {
            lock (this._stateTransition)
            {
                this._unsubscriber?.Dispose();

                this._stockStream = null;
            }
        }
    }
}