using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
{
    public class BmllDataRequestsSenderManager : IBmllDataRequestsSenderManager
    {
        private readonly IBmllTimeBarApiRepository _timeBarRepository;
        private readonly ILogger<BmllDataRequestsSenderManager> _logger;

        public BmllDataRequestsSenderManager(
            IBmllTimeBarApiRepository timeBarRepository,
            ILogger<BmllDataRequestsSenderManager> logger)
        {
            _timeBarRepository = timeBarRepository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<IGetTimeBarPair>> Send(List<MarketDataRequestDataSource> bmllRequests)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                _logger.LogInformation($"BmllDataRequestsSenderManager received a null or empty requests collection. Returning.");
                return new IGetTimeBarPair[0];
            }

            var minuteBarRequests = bmllRequests.Select(GetMinuteBarsRequest).Where(i => i != null).ToList();

            if (!minuteBarRequests.Any())
            {
                _logger.LogError($"BmllDataRequestsManager received {bmllRequests.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

                return new IGetTimeBarPair[0];
            }

            var cts = new CancellationTokenSource(1000 * 60 * 30);
            var checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);

            while (!checkHeartbeat)
            {
                if (cts.IsCancellationRequested)
                {
                    _logger.LogError($"BmllDataRequestsSenderManager ran out of time to connect to the API. Returning an empty response.");
                    return new IGetTimeBarPair[0];
                }

                _logger.LogError($"BmllDataRequestsSenderManager could not elicit a successful heartbeat response. Waiting for a maximum of 30 minutes...");
                Thread.Sleep(1000 * 30);
                checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);
            }

            var timeBarResponses = new List<IGetTimeBarPair>();

            foreach (var req in minuteBarRequests)
            {
                var response = await _timeBarRepository.Get(req);
                var pair = new GetTimeBarPair(req, response);
                timeBarResponses.Add(pair);
            }

            return timeBarResponses;
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MarketDataRequestDataSource request)
        {
            if (request == null
                || (!request.DataRequest?.IsValid() ?? true))
            {
                _logger.LogError($"BmllDataRequestsSenderManager had a null request or a request that did not pass data request validation for {request?.DataRequest?.Identifiers}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.DataRequest.Identifiers.Figi))
            {
                _logger.LogError($"BmllDataRequestsSenderManager asked to process a security without a figi");

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
