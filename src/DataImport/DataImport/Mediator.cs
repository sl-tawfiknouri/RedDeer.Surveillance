namespace DataImport
{
    using System;

    using DataImport.Disk_IO.AllocationFile.Interfaces;
    using DataImport.Disk_IO.EtlFile.Interfaces;
    using DataImport.Disk_IO.Interfaces;
    using DataImport.File_Scanner.Interfaces;
    using DataImport.Interfaces;
    using DataImport.S3_IO.Interfaces;
    using DataImport.Services.Interfaces;

    using Microsoft.Extensions.Logging;

    public class Mediator : IMediator
    {
        private readonly IUploadAllocationFileMonitor _allocationFileMonitor;

        private readonly IEnrichmentService _enrichmentService;

        private readonly IUploadEtlFileMonitor _etlFileMonitor;

        private readonly IFileScannerScheduler _fileScanner;

        private readonly ILogger _logger;

        private readonly IS3FileUploadMonitoringProcess _s3FileUploadProcess;

        private readonly IUploadTradeFileMonitor _tradeFileMonitor;

        public Mediator(
            IEnrichmentService enrichmentService,
            IUploadAllocationFileMonitor allocationFileMonitor,
            IUploadTradeFileMonitor tradeFileMonitor,
            IUploadEtlFileMonitor etlFileMonitor,
            IS3FileUploadMonitoringProcess s3FileUploadProcess,
            IFileScannerScheduler fileScanner,
            ILogger<Mediator> logger)
        {
            this._enrichmentService = enrichmentService ?? throw new ArgumentNullException(nameof(enrichmentService));

            this._allocationFileMonitor =
                allocationFileMonitor ?? throw new ArgumentNullException(nameof(allocationFileMonitor));

            this._tradeFileMonitor = tradeFileMonitor ?? throw new ArgumentNullException(nameof(tradeFileMonitor));

            this._etlFileMonitor = etlFileMonitor ?? throw new ArgumentNullException(nameof(etlFileMonitor));

            this._s3FileUploadProcess =
                s3FileUploadProcess ?? throw new ArgumentNullException(nameof(s3FileUploadProcess));

            this._fileScanner = fileScanner ?? throw new ArgumentNullException(nameof(fileScanner));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            try
            {
                this._logger.LogInformation("Initiating data import in mediator");

                this._enrichmentService.Initialise();
                this._tradeFileMonitor.Initiate();
                this._allocationFileMonitor.Initiate();
                this._etlFileMonitor.Initiate();
                this._fileScanner.Initialise();
                this._s3FileUploadProcess.Initialise(
                    this._allocationFileMonitor,
                    this._tradeFileMonitor,
                    this._etlFileMonitor);

                this._logger.LogInformation("Completed initiating data import in mediator");
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Mediator exception");
            }
        }
    }
}