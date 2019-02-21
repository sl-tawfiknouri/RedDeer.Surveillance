using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.DataSources;
using DataSynchroniser.DataSources.Interfaces;
using DataSynchroniser.Manager.Bmll.Interfaces;
using DataSynchroniser.Manager.Factset.Interfaces;
using DataSynchroniser.Manager.Interfaces;
using Domain.Markets;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IDataSourceClassifier _dataSourceClassifier;
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly IBmllDataRequestManager _bmllDataRequestManager;
        private readonly IFactsetDataRequestsManager _factsetDataRequestManager;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IDataSourceClassifier dataSourceClassifier,
            IRuleRunDataRequestRepository dataRequestRepository,
            IBmllDataRequestManager bmllDataRequestManager,
            IFactsetDataRequestsManager factsetDataRequestManager,
            ILogger<DataRequestManager> logger)
        {
            _dataSourceClassifier = dataSourceClassifier ?? throw new ArgumentNullException(nameof(dataSourceClassifier));
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _bmllDataRequestManager = bmllDataRequestManager ?? throw new ArgumentNullException(nameof(bmllDataRequestManager));
            _factsetDataRequestManager = factsetDataRequestManager ?? throw new ArgumentNullException(nameof(factsetDataRequestManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(string systemProcessOperationId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext)
        {
            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger.LogError($"DataRequestManager asked to handle a systemProcessOperationId that had a null or empty id");
                dataRequestContext.EventError($"DataRequestManager systemProcessOperationId was null");
                return;
            }

            _logger.LogInformation($"DataRequestManager handling request with id {systemProcessOperationId}");

            var dataRequests = await _dataRequestRepository.DataRequestsForSystemOperation(systemProcessOperationId);
            var dataRequestWithSource = dataRequests.Select(CalculateDataSource).GroupBy(i => i.DataSource).ToList();

            var bmllRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Bmll)?.ToList();
            var markitRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Markit)?.ToList();
            var otherRequests = dataRequestWithSource.Where(i => i.Key != DataSource.Bmll && i.Key != DataSource.Markit).SelectMany(i => i).ToList();

            await SubmitToBmllAndFactset(systemProcessOperationId, bmllRequests);
            SubmitToMarkit(markitRequests);
            SubmitOther(otherRequests);

            _logger.LogInformation($"DataRequestManager completed handling request with id {systemProcessOperationId}");
        }

        private MarketDataRequestDataSource CalculateDataSource(MarketDataRequest request)
        {
            var source = _dataSourceClassifier.Classify(request.Cfi);

            return new MarketDataRequestDataSource(source, request);
        }

        /// <summary>
        /// BMLL does not provide the full set of daily summary data so we have to fetch this from fact set
        /// </summary>
        private async Task SubmitToBmllAndFactset(string systemProcessOperationId, List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null)
            {
                return;
            }

            if (!bmllRequests.Any())
            {
                return;
            }

            _logger.LogInformation($"DataRequestManager received {bmllRequests.Count} market data requests for BMLL (not deduplicated)");

            // does not reschedule
            await _factsetDataRequestManager.Submit(bmllRequests);

            // performs rescheduling as a side effect
            await _bmllDataRequestManager.Submit(systemProcessOperationId, bmllRequests);
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
