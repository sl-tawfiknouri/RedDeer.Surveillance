using System;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Interfaces;
using DataImport.S3_IO.Interfaces;
using DataImport.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport
{
    public class Mediator : IMediator
    {
        private readonly IEnrichmentService _enrichmentService;
        private readonly IUploadAllocationFileMonitor _allocationFileMonitor;
        private readonly IUploadTradeFileMonitor _tradeFileMonitor;
        private readonly IS3FileUploadMonitoringProcess _s3FileUploadProcess;
        private readonly ILogger _logger;

        public Mediator(
            IEnrichmentService enrichmentService,
            IUploadAllocationFileMonitor allocationFileMonitor,
            IUploadTradeFileMonitor tradeFileMonitor,
            IS3FileUploadMonitoringProcess s3FileUploadProcess,
            ILogger<Mediator> logger)
        {
            _enrichmentService =
                enrichmentService
                ?? throw new ArgumentNullException(nameof(enrichmentService));

            _allocationFileMonitor =
                allocationFileMonitor
                ?? throw new ArgumentNullException(nameof(allocationFileMonitor));

            _tradeFileMonitor =
                tradeFileMonitor
                ?? throw new ArgumentNullException(nameof(tradeFileMonitor));

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
                _tradeFileMonitor.Initiate();
                _allocationFileMonitor.Initiate();

                _s3FileUploadProcess.Initialise(_allocationFileMonitor, _tradeFileMonitor);

                _logger.LogInformation("Completed initiating data import in mediator");
            }
            catch (Exception e)
            {
                _logger.LogError($"Mediator exception {e.Message} - {e?.InnerException?.Message}");
            }
        }
    }
}