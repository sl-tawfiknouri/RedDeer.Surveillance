using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IBmllDataRequestRepository _dataRequestRepository;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IBmllDataRequestRepository dataRequestRepository,
            ILogger<DataRequestManager> logger)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(string ruleRunId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext)
        {
            if (string.IsNullOrWhiteSpace(ruleRunId))
            {
                _logger.LogError($"DataRequestManager asked to handle a rule run that had a null or empty id");
                dataRequestContext.EventError($"DataRequestManager RuleRunId was null");
                return;
            }

            _logger.LogInformation($"DataRequestManager handling request with id {ruleRunId}");

            var dataRequests = await _dataRequestRepository.DataRequestsForRuleRun(ruleRunId);

            _logger.LogInformation($"DataRequestManager completed handling request with id {ruleRunId}");
        }
    }
}
