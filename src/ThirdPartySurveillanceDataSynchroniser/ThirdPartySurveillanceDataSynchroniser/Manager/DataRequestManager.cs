using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.DataSources;
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
            var dataRequestWithSource = dataRequests.Select(CalculateDataSource).GroupBy(i => i.DataSource).ToList();

            var bmllRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Bmll)?.ToList();
            var markitRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Markit)?.ToList();
            var otherRequests = dataRequestWithSource.Where(i => i.Key != DataSource.Bmll && i.Key != DataSource.Markit).SelectMany(i => i).ToList();

            SubmitToBmll(bmllRequests);
            SubmitToMarkit(markitRequests);
            SubmitOther(otherRequests);

            _logger.LogInformation($"DataRequestManager completed handling request with id {ruleRunId}");
        }

        private MarketDataRequestDataSource CalculateDataSource(MarketDataRequest request)
        {
            var source = _dataSourceClassifier.Classify(request.Cfi);

            return new MarketDataRequestDataSource(source, request);
        }

        private void SubmitToBmll(List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null)
            {
                return;
            }

            if (!bmllRequests.Any())
            {
                return;
            }

            _logger.LogError($"DataRequestManager received {bmllRequests.Count} market data requests for BMLL (not deduplicated)");
        }

        private void SubmitToMarkit(List<MarketDataRequestDataSource> markitRequests)
        {
            if (markitRequests == null)
            {
                return;
            }

            if (!markitRequests.Any())
            {
                return;
            }

            _logger.LogError($"DataRequestManager received {markitRequests.Count} market data requests for MARKIT which we have not implemented yet");
        }

        private void SubmitOther(List<MarketDataRequestDataSource> otherRequests)
        {
            if (otherRequests == null)
            {
                return;
            }

            if (!otherRequests.Any())
            {
                return;
            }

            _logger.LogError($"DataRequestManager received {otherRequests.Count} market data requests we do not have the necessary data suppliers for");
        }
    }
}
