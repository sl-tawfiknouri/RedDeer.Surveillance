using DataImport.Configuration.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using System;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.AllocationFile
{
    public class AllocationFileMonitorFactory : IAllocationFileMonitorFactory
    {
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly ILogger<AllocationFileMonitor> _logger;

        public AllocationFileMonitorFactory(
            ISystemProcessContext systemProcessContext,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory reddeerDirectory,
            ILogger<AllocationFileMonitor> logger)
        {
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _reddeerDirectory = reddeerDirectory ?? throw new ArgumentNullException(nameof(reddeerDirectory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IAllocationFileMonitor Build()
        {
            _logger.LogInformation($"AllocationFileMonitorFactory Build method called");
            return new AllocationFileMonitor(
                _systemProcessContext,
                _uploadConfiguration,
                _reddeerDirectory,
                _logger);
        }
    }
}
