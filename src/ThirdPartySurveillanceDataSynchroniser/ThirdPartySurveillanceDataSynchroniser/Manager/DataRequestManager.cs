using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using DataSynchroniser.Api.Markit.Interfaces;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Queues.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IBmllDataSynchroniser _bmllSynchroniser;
        private readonly IFactsetDataSynchroniser _factsetSynchroniser;
        private readonly IMarkitDataSynchroniser _markitSynchroniser;
        private readonly IScheduleRulePublisher _rulePublisher;
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IBmllDataSynchroniser bmllSynchroniser,
            IFactsetDataSynchroniser factsetSynchroniser,
            IMarkitDataSynchroniser markitSynchroniser,
            IScheduleRulePublisher rulePublisher,
            IRuleRunDataRequestRepository dataRequestRepository,
            ILogger<DataRequestManager> logger)
        {
            _bmllSynchroniser = bmllSynchroniser ?? throw new ArgumentNullException(nameof(bmllSynchroniser));
            _factsetSynchroniser = factsetSynchroniser ?? throw new ArgumentNullException(nameof(factsetSynchroniser));
            _markitSynchroniser = markitSynchroniser ?? throw new ArgumentNullException(nameof(markitSynchroniser));
            _rulePublisher = rulePublisher ?? throw new ArgumentNullException(nameof(rulePublisher));
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(string systemProcessOperationId, ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext)
        {

            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                _logger.LogError($"{nameof(DataRequestManager)} asked to handle a systemProcessOperationId that had a null or empty id");
                dataRequestContext.EventError($"DataRequestManager systemProcessOperationId was null");
                return;
            }

            _logger.LogInformation($"{nameof(DataRequestManager)} handling request with id {systemProcessOperationId}");

            var dataRequests = await _dataRequestRepository.DataRequestsForSystemOperation(systemProcessOperationId);
            dataRequests = dataRequests ?? new List<MarketDataRequest>();
            _logger.LogInformation($"handling request with id {systemProcessOperationId} had {dataRequests.Count} data requests to process across all data sources");

            try
            {
                // Equity handling
                var factsetData = dataRequests.Where(_ => _.DataSource == DataSource.Factset || _.DataSource == DataSource.All || _.DataSource == DataSource.AllInterday).ToList();
                _logger.LogInformation($"handling request with id {systemProcessOperationId} had {factsetData.Count} factset data requests to process");
                if (factsetData.Any())
                {
                    await _factsetSynchroniser.Handle(systemProcessOperationId, dataRequestContext, factsetData);
                }

                var bmllData = dataRequests.Where(_ => _.DataSource == DataSource.Bmll || _.DataSource == DataSource.All || _.DataSource == DataSource.AllIntraday).ToList();
                _logger.LogInformation($"handling request with id {systemProcessOperationId} had {bmllData.Count} bmll data requests to process");
                if (bmllData.Any())
                {
                    await _bmllSynchroniser.Handle(systemProcessOperationId, dataRequestContext, bmllData);
                }

                // Fixed income handling
                var markitData = dataRequests.Where(_ => _.DataSource == DataSource.Markit || _.DataSource == DataSource.All || _.DataSource == DataSource.AllInterday || _.DataSource == DataSource.AllIntraday).ToList();
                _logger.LogInformation($"handling request with id {systemProcessOperationId} had {markitData.Count} markit data requests to process");
                if (markitData.Any())
                {
                    await _markitSynchroniser.Handle(systemProcessOperationId, dataRequestContext, markitData);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (dataRequests != null
                    && dataRequests.Any())
                {
                    await _rulePublisher.RescheduleRuleRun(systemProcessOperationId, dataRequests);
                }

                _logger.LogInformation($"{nameof(DataRequestManager)} completed handling request with id {systemProcessOperationId}");
            }
        }
    }
}
