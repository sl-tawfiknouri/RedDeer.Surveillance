﻿using System;
using DataImport.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.S3_IO.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport
{
    public class Mediator : IMediator
    {
        private readonly ITradeOrderStreamManager _tradeOrderStreamManager;
        private readonly IStockExchangeStreamManager _stockExchangeStreamManager;
        private readonly IS3FileUploadMonitoringProcess _s3FileUploadProcess;
        private readonly ILogger _logger;

        public Mediator(
            ITradeOrderStreamManager tradeOrderStreamManager,
            IStockExchangeStreamManager stockExchangeStreamManager,
            IS3FileUploadMonitoringProcess s3FileUploadProcess,
            ILogger<Mediator> logger)
        {
            _tradeOrderStreamManager =
                tradeOrderStreamManager
                ?? throw new ArgumentNullException(nameof(tradeOrderStreamManager));

            _stockExchangeStreamManager =
                stockExchangeStreamManager
                ?? throw new ArgumentNullException(nameof(stockExchangeStreamManager));

            _s3FileUploadProcess =
                s3FileUploadProcess
                ?? throw new ArgumentNullException(nameof(s3FileUploadProcess));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            try
            {
                _logger.LogInformation("Initiating relay in mediator");

                var tradeFileMonitor = _tradeOrderStreamManager.Initialise();
                var equityFileMonitor = _stockExchangeStreamManager.Initialise();
                _s3FileUploadProcess.Initialise(tradeFileMonitor, equityFileMonitor);

                _logger.LogInformation("Completed initiating relay in mediator");
            }
            catch (Exception e)
            {
                _logger.LogError($"Mediator exception {e.Message} - {e?.InnerException?.Message}");
            }
        }
    }
}