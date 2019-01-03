using System;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsStorageManager : IBmllDataRequestsStorageManager
    {
        private readonly ILogger<BmllDataRequestsStorageManager> _logger;

        public BmllDataRequestsStorageManager(
            ILogger<BmllDataRequestsStorageManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Store()
        {
            _logger.LogInformation($"BmllDataRequestsStorageManager beginning storage process for BMLL response data");

            _logger.LogInformation($"BmllDataRequestsStorageManager completed storage process for BMLL response data");
        }
    }
}
