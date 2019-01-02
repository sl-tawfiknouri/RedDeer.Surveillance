using System;
using DataImport.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.S3_IO.Interfaces;
using DataImport.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport
{
    public class Mediator : IMediator
    {
        private readonly IEnrichmentService _enrichmentService;
        private readonly ITradeOrderStreamManager _tradeOrderStreamManager;
        private readonly IStockExchangeStreamManager _stockExchangeStreamManager;
        private readonly IS3FileUploadMonitoringProcess _s3FileUploadProcess;
        private readonly ILogger _logger;

        public Mediator(
            IEnrichmentService enrichmentService,
            ITradeOrderStreamManager tradeOrderStreamManager,
            IStockExchangeStreamManager stockExchangeStreamManager,
            IS3FileUploadMonitoringProcess s3FileUploadProcess,
            ILogger<Mediator> logger)
        {
            _enrichmentService =
                enrichmentService
                ?? throw new ArgumentNullException(nameof(enrichmentService));

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
                _logger.LogInformation("Initiating data import in mediator");

                _enrichmentService.Initialise();
                var tradeFileMonitor = _tradeOrderStreamManager.Initialise();
                var equityFileMonitor = _stockExchangeStreamManager.Initialise();
                _s3FileUploadProcess.Initialise(tradeFileMonitor, equityFileMonitor);

                _logger.LogInformation("Completed initiating data import in mediator");
            }
            catch (Exception e)
            {
                _logger.LogError($"Mediator exception {e.Message} - {e?.InnerException?.Message}");
            }
        }
    }
}