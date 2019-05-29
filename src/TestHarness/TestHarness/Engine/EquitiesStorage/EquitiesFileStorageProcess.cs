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

namespace TestHarness.Engine.EquitiesStorage
{
    public class EquitiesFileStorageProcess : IEquityDataStorage
    {
        private IDisposable _unsubscriber;
        private IStockExchangeStream _stockStream;
        private readonly object _stateTransition = new object();

        private readonly string _path;
        private readonly ILogger _logger;
        private readonly IDtoToSecurityCsvMapper _securityMapper;

        public EquitiesFileStorageProcess(
            string path,
            ILogger logger,
            IDtoToSecurityCsvMapper securityMapper)
        {
            _path = path ?? string.Empty;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _securityMapper = securityMapper ?? throw new ArgumentNullException(nameof(securityMapper));
        }

        public void Initiate(IStockExchangeStream stockStream)
        {
            lock (_stateTransition)
            {
                if (stockStream == null)
                {
                    _logger.Log(LogLevel.Error, "Initiation attempt in order data generator with null stock stream");
                    return;
                }

                _stockStream = stockStream;
                _unsubscriber = stockStream.Subscribe(this);
            }

            if (string.IsNullOrWhiteSpace(_path))
            {
                _logger.LogError("Equities File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public void OnCompleted()
        {
            _logger.LogInformation("Order data generator received completed message from stock stream. Terminating equities file storage");
            Terminate();            
        }

        public void OnError(Exception error)
        {
            _logger.LogError(error.Message);
        }

        public void OnNext(EquityIntraDayTimeBarCollection value)
        {
            var csvRecords = value?
                .Securities?
                .Select(_securityMapper.Map)
                .Where(w => w != null)
                .ToList() ?? new List<FinancialInstrumentTimeBarCsv>();

            if (!csvRecords.Any())
            {
                return;
            }

            var fileName = $"{value.Exchange.Id}-{value.Epoch.ToString("yyyyMMddHHmmssffff")}.csv";
            var filePath = Path.Combine(_path, fileName);

            using (var writer = File.CreateText(filePath))
            {
                var csv = new CsvWriter(writer);
                csv.Configuration.HasHeaderRecord = true;
                csv.WriteRecords<FinancialInstrumentTimeBarCsv>(csvRecords);
            }
        }

        /// <summary>
        /// Avoid calling this from inside another state transition
        /// </summary>
        public void Terminate()
        {
            lock (_stateTransition)
            {
                _unsubscriber?.Dispose();

                _stockStream = null;
            }
        }
    }
}
