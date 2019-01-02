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
        private readonly ILogger<BmllDataRequestsManager> _logger;

        public BmllDataRequestsManager(
            ILogger<BmllDataRequestsManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null)
            {
                return;
            }

            _logger.LogInformation($"BmllDataRequestsManager received {bmllRequests.Count} data requests");

            var minuteBarRequests = bmllRequests.Select(GetMinuteBarsRequest).ToList();

            foreach (var req in minuteBarRequests)
            {
                var cts = new CancellationTokenSource();
                // var result = await _bus.RequestAsync<GetMinuteBarsRequest, GetMinuteBarsResponse>(req, cts.Token);
            }

            _logger.LogInformation($"BmllDataRequestsManager has completed submission of {bmllRequests.Count} requests");
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MarketDataRequestDataSource request)
        {
            if (request == null
                || !request.DataRequest.IsValid())
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
