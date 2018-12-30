using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IDataSourceClassifier _dataSourceClassifier;
        private readonly IBmllDataRequestRepository _dataRequestRepository;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IDataSourceClassifier dataSourceClassifier,
            IBmllDataRequestRepository dataRequestRepository,
            ILogger<DataRequestManager> logger)
        {
            _dataSourceClassifier = dataSourceClassifier ?? throw new ArgumentNullException(nameof(dataSourceClassifier));
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



            // alright so this can be beefed up a bit but lets save that fur laters
            // i.e. creating proper time segments of requests per client id

            // ok! so getting figis into enrichment is building
            // whilst that's churning away lets roll on with this
            // so I now have a list of data requests, first get their data source

            _logger.LogInformation($"DataRequestManager completed handling request with id {ruleRunId}");
        }
    }
}
