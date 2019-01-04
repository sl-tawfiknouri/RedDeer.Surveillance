using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsStorageManager : IBmllDataRequestsStorageManager
    {
        private readonly ILogger<BmllDataRequestsStorageManager> _logger;

        public BmllDataRequestsStorageManager(ILogger<BmllDataRequestsStorageManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs)
        {
            _logger.LogInformation($"BmllDataRequestsStorageManager beginning storage process for BMLL response data");

            if (timeBarPairs == null
                || !timeBarPairs.Any())
            {
                _logger.LogInformation($"BmllDataRequestsStorageManager completed storage process for BMLL response data as it had nothing to store.");
                return;
            }

            // ok so now just save the time bars into market data repository



            _logger.LogInformation($"BmllDataRequestsStorageManager completed storage process for BMLL response data");
        }
    }
}
