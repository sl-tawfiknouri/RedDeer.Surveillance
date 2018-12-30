using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.BmllSubmissons.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.BmllSubmissons
{
    public class BmllDataRequestsManager : IBmllDataRequestManager
    {
        private readonly ILogger<BmllDataRequestsManager> _logger;

        public BmllDataRequestsManager(ILogger<BmllDataRequestsManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Submit(List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null)
            {
                return;
            }

            _logger.LogInformation($"BmllDataRequestsManager received {bmllRequests.Count} data requests");



            _logger.LogInformation($"BmllDataRequestsManager has completed submission of {bmllRequests.Count} requests");
        }
    }
}
