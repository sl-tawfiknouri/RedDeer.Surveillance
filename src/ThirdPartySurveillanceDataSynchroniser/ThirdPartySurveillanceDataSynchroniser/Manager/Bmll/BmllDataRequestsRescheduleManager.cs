using System;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsRescheduleManager : IBmllDataRequestsRescheduleManager
    {
        private readonly ILogger<BmllDataRequestsRescheduleManager> _logger;

        public BmllDataRequestsRescheduleManager(
            ILogger<BmllDataRequestsRescheduleManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void RescheduleRuleRun()
        {
            _logger?.LogInformation($"RescheduleRuleRun beginning process");



            _logger?.LogInformation($"RescheduleRuleRun completing process");
        }
    }
}
