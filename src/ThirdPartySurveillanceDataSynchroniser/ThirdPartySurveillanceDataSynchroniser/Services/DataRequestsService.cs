using System;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Services
{
    public class DataRequestsService : IDataRequestsService
    {
        private readonly ILogger<DataRequestsService> _logger;

        public DataRequestsService(ILogger<DataRequestsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"DataRequestsService initiate beginning");

            _logger.LogInformation($"DataRequestsService initiate completed");
        }

        public void Terminate()
        {
            _logger.LogInformation($"DataRequestsService terminate beginning");

            _logger.LogInformation($"DataRequestsService terminate completed");
        }
    }
}
