using System;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using DataSynchroniser.Api.Markit.Interfaces;
using DataSynchroniser.Manager.Interfaces;
using DataSynchroniser.Queues.Interfaces;
using Microsoft.Extensions.Logging;
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
            
            // Equity handling
            await _factsetSynchroniser.Handle(systemProcessOperationId, dataRequestContext, dataRequests);
            await _bmllSynchroniser.Handle(systemProcessOperationId, dataRequestContext, dataRequests);

            // Fixed income handling
            await _markitSynchroniser.Handle(systemProcessOperationId, dataRequestContext, dataRequests);

            await _rulePublisher.RescheduleRuleRun(systemProcessOperationId, dataRequests);

            _logger.LogInformation($"{nameof(DataRequestManager)} completed handling request with id {systemProcessOperationId}");
        }
    }
}
