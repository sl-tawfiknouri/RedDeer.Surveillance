using System;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Bmll.Interfaces;
using DataSynchroniser.Manager.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Manager
{
    public class DataRequestManager : IDataRequestManager
    {
        private readonly IRuleRunDataRequestRepository _dataRequestRepository;
        private readonly IBmllDataRequestManager _bmllDataRequestManager;
        private readonly ILogger<DataRequestManager> _logger;

        public DataRequestManager(
            IRuleRunDataRequestRepository dataRequestRepository,
            IBmllDataRequestManager bmllDataRequestManager,
            ILogger<DataRequestManager> logger)
        {
            _dataRequestRepository = dataRequestRepository ?? throw new ArgumentNullException(nameof(dataRequestRepository));
            _bmllDataRequestManager = bmllDataRequestManager ?? throw new ArgumentNullException(nameof(bmllDataRequestManager));
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





            //var bmllRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Bmll)?.ToList();
            //var markitRequests = dataRequestWithSource.FirstOrDefault(i => i.Key == DataSource.Markit)?.ToList();
            //var otherRequests = dataRequestWithSource.Where(i => i.Key != DataSource.Bmll && i.Key != DataSource.Markit).SelectMany(i => i).ToList();

            //await SubmitToBmllAndFactset(systemProcessOperationId, bmllRequests);
            //SubmitToMarkit(markitRequests);
            //SubmitOther(otherRequests);

            _logger.LogInformation($"DataRequestManager completed handling request with id {systemProcessOperationId}");
        }

        ///// <summary>
        ///// BMLL does not provide the full set of daily summary data so we have to fetch this from fact set
        ///// </summary>
        //private async Task SubmitToBmllAndFactset(string systemProcessOperationId, List<MarketDataRequest> bmllRequests)
        //{
        //    if (bmllRequests == null)
        //    {
        //        return;
        //    }

        //    if (!bmllRequests.Any())
        //    {
        //        return;
        //    }

        //    _logger.LogInformation($"DataRequestManager received {bmllRequests.Count} market data requests for BMLL (not deduplicated)");

        //    // does not reschedule
        //    await _factsetDataRequestManager.Submit(bmllRequests);

        //    // performs rescheduling as a side effect
        //    await _bmllDataRequestManager.Submit(systemProcessOperationId, bmllRequests);
        //}

        //private void SubmitToMarkit(List<MarketDataRequest> markitRequests)
        //{
        //    if (markitRequests == null)
        //    {
        //        return;
        //    }

        //    if (!markitRequests.Any())
        //    {
        //        return;
        //    }
            
        //    _logger.LogError($"DataRequestManager received {markitRequests.Count} market data requests for MARKIT which we have not implemented yet");
        //}

        //private void SubmitOther(List<MarketDataRequest> otherRequests)
        //{
        //    if (otherRequests == null)
        //    {
        //        return;
        //    }

        //    if (!otherRequests.Any())
        //    {
        //        return;
        //    }

        //    _logger.LogError($"DataRequestManager received {otherRequests.Count} market data requests we do not have the necessary data suppliers for");
        //}
    }
}
