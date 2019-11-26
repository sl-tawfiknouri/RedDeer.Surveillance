using Surveillance.Data.Universe.Refinitiv.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using DataSynchroniser.Api.Markit.Interfaces;
using DataSynchroniser.Api.Refinitive.Interfaces;
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

        private readonly IRuleRunDataRequestRepository _dataRequestRepository;

        private readonly IFactsetDataSynchroniser _factsetSynchroniser;

        private readonly ILogger<DataRequestManager> _logger;

        private readonly IMarkitDataSynchroniser _markitSynchroniser;

        private readonly IScheduleRulePublisher _rulePublisher;
        
        private readonly IRefinitivDataSynchroniser _refinitivDataSynchroniser;
        
        public DataRequestManager(
            IBmllDataSynchroniser bmllSynchroniser,
            IFactsetDataSynchroniser factsetSynchroniser,
            IMarkitDataSynchroniser markitSynchroniser,
            IScheduleRulePublisher rulePublisher,
            IRuleRunDataRequestRepository dataRequestRepository,
            IRefinitivDataSynchroniser refinitivDataSynchroniser,
            ILogger<DataRequestManager> logger)
        {
            this._bmllSynchroniser = bmllSynchroniser ?? throw new ArgumentNullException(nameof(bmllSynchroniser));
            this._factsetSynchroniser = factsetSynchroniser ?? throw new ArgumentNullException(nameof(factsetSynchroniser));
            this._markitSynchroniser = markitSynchroniser ?? throw new ArgumentNullException(nameof(markitSynchroniser));
            this._rulePublisher = rulePublisher ?? throw new ArgumentNullException(nameof(rulePublisher));
            this._dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            this._refinitivDataSynchroniser = refinitivDataSynchroniser ?? throw new ArgumentNullException(nameof(refinitivDataSynchroniser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(
            string systemProcessOperationId,
            ISystemProcessOperationThirdPartyDataRequestContext dataRequestContext)
        {
            if (string.IsNullOrWhiteSpace(systemProcessOperationId))
            {
                this._logger.LogError(
                    $"{nameof(DataRequestManager)} asked to handle a systemProcessOperationId that had a null or empty id");
                dataRequestContext.EventError("DataRequestManager systemProcessOperationId was null");
                return;
            }

            this._logger.LogInformation(
                $"{nameof(DataRequestManager)} handling request with id {systemProcessOperationId}");

            var dataRequests =
                await this._dataRequestRepository.DataRequestsForSystemOperation(systemProcessOperationId);
            dataRequests = dataRequests ?? new List<MarketDataRequest>();
            this._logger.LogInformation(
                $"handling request with id {systemProcessOperationId} had {dataRequests.Count} data requests to process across all data sources");

            try
            {
                // Equity handling
                var factsetData = dataRequests.Where(
                    _ => _.DataSource == DataSource.Factset || _.DataSource == DataSource.Any
                                                            || _.DataSource == DataSource.AnyInterday).ToList();
                this._logger.LogInformation(
                    $"handling request with id {systemProcessOperationId} had {factsetData.Count} factset data requests to process");
                if (factsetData.Any())
                    await this._factsetSynchroniser.Handle(systemProcessOperationId, dataRequestContext, factsetData);

                var bmllData = dataRequests.Where(
                    _ => _.DataSource == DataSource.Bmll || _.DataSource == DataSource.Any
                                                         || _.DataSource == DataSource.AnyIntraday).ToList();
                this._logger.LogInformation(
                    $"handling request with id {systemProcessOperationId} had {bmllData.Count} bmll data requests to process");
                if (bmllData.Any())
                    await this._bmllSynchroniser.Handle(systemProcessOperationId, dataRequestContext, bmllData);

                // Fixed income handling
                var refinitivInterdayData = dataRequests.Where(_ => _.DataSource == DataSource.RefinitivInterday).ToList();
                
                this._logger.LogInformation($"handling request with id {systemProcessOperationId} had {refinitivInterdayData.Count} refinitiv data requests to process");
                
                if (refinitivInterdayData.Any())
                    await this._refinitivDataSynchroniser.Handle(systemProcessOperationId, dataRequestContext, refinitivInterdayData);

                var markitData = dataRequests.Where(
                    _ => _.DataSource == DataSource.Markit || _.DataSource == DataSource.Any
                                                           || _.DataSource == DataSource.AnyInterday
                                                           || _.DataSource == DataSource.AnyIntraday).ToList();
                this._logger.LogInformation(
                    $"handling request with id {systemProcessOperationId} had {markitData.Count} markit data requests to process");
                if (markitData.Any())
                    await this._markitSynchroniser.Handle(systemProcessOperationId, dataRequestContext, markitData);
            }
            finally
            {
                if (dataRequests != null && dataRequests.Any())
                    await this._rulePublisher.RescheduleRuleRun(systemProcessOperationId, dataRequests);

                this._logger.LogInformation(
                    $"{nameof(DataRequestManager)} completed handling request with id {systemProcessOperationId}");
            }
        }
    }
}