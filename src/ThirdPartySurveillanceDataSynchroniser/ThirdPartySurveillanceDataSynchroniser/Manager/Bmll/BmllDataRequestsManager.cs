using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsManager : IBmllDataRequestManager
    {
        private readonly IBmllDataRequestsStorageManager _storageManager;
        private readonly IBmllDataRequestsRescheduleManager _rescheduleManager;
        private readonly ILogger<BmllDataRequestsManager> _logger;

        public BmllDataRequestsManager(
            IBmllDataRequestsStorageManager storageManager,
            IBmllDataRequestsRescheduleManager rescheduleManager,
            ILogger<BmllDataRequestsManager> logger)
        {
            _storageManager = storageManager ?? throw new ArgumentNullException(nameof(storageManager));
            _rescheduleManager = rescheduleManager ?? throw new ArgumentNullException(nameof(rescheduleManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                return;
            }

            _logger.LogInformation($"BmllDataRequestsManager received {bmllRequests.Count} data requests");

            var minuteBarRequests = bmllRequests.Select(GetMinuteBarsRequest).Where(i => i != null).ToList();

            if (!minuteBarRequests.Any())
            {
                _logger.LogError($"BmllDataRequestsManager received {bmllRequests.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

                return;
            }

            foreach (var req in minuteBarRequests)
            {
                var cts = new CancellationTokenSource();
                // var result = await _bus.RequestAsync<GetMinuteBarsRequest, GetMinuteBarsResponse>(req, cts.Token);
            }

            // STORE IT
            await _storageManager.Store();
            
            // RESCHEDULE IT
            await _rescheduleManager.RescheduleRuleRun(bmllRequests);

            _logger.LogInformation($"BmllDataRequestsManager has completed submission of {bmllRequests.Count} requests");
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MarketDataRequestDataSource request)
        {
            if (request == null
                || (!request.DataRequest?.IsValid() ?? true))
            {
                _logger.LogError($"BmllDataRequestManager had a null request or a request that did not pass data request validation for {request?.DataRequest?.Identifiers}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.DataRequest.Identifiers.Figi))
            {
                _logger.LogError($"BmllDataRequestsManager asked to process a security without a figi");

                return null;
            }

            return new GetMinuteBarsRequest
            {
                Figi = request.DataRequest.Identifiers.Figi,
                From = request.DataRequest.UniverseEventTimeFrom.Value.Date,
                To = request.DataRequest.UniverseEventTimeTo.Value.Date.AddDays(1).AddMilliseconds(-1),
                Interval = "1min",
            };
        }
    }
}
