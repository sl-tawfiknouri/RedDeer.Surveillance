using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using System;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.AllocationFile
{
    public class AllocationFileMonitorFactory : IAllocationFileMonitorFactory
    {
        private readonly IAllocationFileProcessor _fileProcessor;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly ILogger<AllocationFileMonitor> _logger;

        public AllocationFileMonitorFactory(
            IAllocationFileProcessor fileProcessor,
            ISystemProcessContext systemProcessContext,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory reddeerDirectory,
            ILogger<AllocationFileMonitor> logger)
        {
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _reddeerDirectory = reddeerDirectory ?? throw new ArgumentNullException(nameof(reddeerDirectory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUploadAllocationFileMonitor Create(IOrderAllocationStream<OrderAllocation> stream)
        {
            _logger.LogInformation($"AllocationFileMonitorFactory Build method called");
            return new AllocationFileMonitor(
                stream,
                _fileProcessor,
                _systemProcessContext,
                _uploadConfiguration,
                _reddeerDirectory,
                _logger);
        }
    }
}
