using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Firefly.Service.Data.BMLL.Shared.Commands;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.BmllMarketData;
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
            _timeBarRepository = timeBarRepository ?? throw new ArgumentNullException(nameof(timeBarRepository)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(
            List<MarketDataRequestDataSource> bmllRequests,
            bool completeWithFailures)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                _logger.LogInformation($"BmllDataRequestsSenderManager received a null or empty requests collection. Returning.");
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            try
            {
                var cts = new CancellationTokenSource(1000 * 60 * 15);
                var checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);

                while (!checkHeartbeat)
                {
                    if (cts.IsCancellationRequested)
                    {
                        _logger.LogError($"BmllDataRequestsSenderManager ran out of time to connect to the API. Returning an empty response.");
                        return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
                    }

                    _logger.LogError($"BmllDataRequestsSenderManager could not elicit a successful heartbeat response. Waiting for a maximum of 30 minutes...");
                    Thread.Sleep(1000 * 30);
                    checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"BmllDataRequestSenderManager Send encountered an error whilst monitoring for heartbeating...", e);
            }

            try
            {
                _logger.LogInformation($"BmllDataRequestSenderManager beginning 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars");

                // step 0.
                // project to request keys
                var keys = ProjectToRequestKeys(bmllRequests);

                if (keys == null
                    || !keys.Any())
                {
                    _logger.LogInformation($"BmllDataRequestSenderManager completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars) at Project Keys. Had no keys to fetch. Returning.");

                    return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
                }

                // step 1.
                // create minute bar request
                await CreateMinuteBarRequest(keys);

                // step 2.
                // loop on the status update polling
                var bmllWorkResult = await BlockUntilBmllWorkIsDone(keys);

                if (bmllWorkResult == BmllStatusMinuteBarResult.CompletedWithFailures
                    && !completeWithFailures)
                {
                    return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(false, new IGetTimeBarPair[0]);
                }

                // step 3.
                // get minute bar request
                var timeBarResponses = GetTimeBars(keys);

                _logger.LogInformation($"BmllDataRequestSenderManager completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars)");

                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, timeBarResponses);
            }
            catch (Exception e)
            {
                _logger?.LogError($"BmllDataRequestSenderManager encountered an unexpected error during processing. {e.Message} {e.InnerException?.Message}");
            }

            return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
        }

        public IReadOnlyCollection<MinuteBarRequestKeyDto> ProjectToRequestKeys(List<MarketDataRequestDataSource> bmllRequests)
        {
            var keys = new List<MinuteBarRequestKeyDto>();

            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                return keys;
            }

            foreach (var req in bmllRequests)
            {
                if (req.DataRequest == null
                    || string.IsNullOrWhiteSpace(req.DataRequest.Identifiers.Figi)
                    || req.DataRequest.UniverseEventTimeTo == null
                    || req.DataRequest.UniverseEventTimeFrom == null)
                {
                    continue;
                }

                var toTarget = req.DataRequest.UniverseEventTimeTo.Value;
                var fromTarget = req.DataRequest.UniverseEventTimeFrom.Value;

                var timeSpan = toTarget.Subtract(fromTarget);
                var totalDays = timeSpan.TotalDays + 1;
                var iter = 0;

                while (iter <= totalDays)
                {
                    var date = fromTarget.AddDays(iter);

                    var barRequest = new MinuteBarRequestKeyDto(req.DataRequest.Identifiers.Figi, "1min", date);
                    keys.Add(barRequest);

                    iter += 1;
                }
            }

            var filteredKeys = keys.Where(key => !string.IsNullOrWhiteSpace(key.Figi)).ToList();

            var deduplicatedKeys = new List<MinuteBarRequestKeyDto>();

            var grps = filteredKeys.GroupBy(x => x.Figi);
            foreach (var grp in grps)
            {
                var dedupe = grp.GroupBy(x => x.Date.Date).Select(x => x.FirstOrDefault()).Where(x => x != null).ToList();

                if (!dedupe.Any())
                {
                    continue;
                }
                
                deduplicatedKeys.AddRange(dedupe);
            }
            
            return deduplicatedKeys;
        }

        private async Task CreateMinuteBarRequest(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"BmllDataRequestSenderManager CreateMinuteBarRequest active");

            var request = new CreateMinuteBarRequestCommand { Keys = keys?.ToList() };

            await _timeBarRepository.RequestMinuteBars(request);

            _logger.LogInformation($"BmllDataRequestSenderManager CreateMinuteBarRequest complete");
        }

        private async Task<BmllStatusMinuteBarResult> BlockUntilBmllWorkIsDone(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"BmllDataRequestSenderManager BlockUntilBmllWorkIsDone active");

            var hasSuccess = false;
            var cts = new CancellationTokenSource(1000 * 60 * 5);
            var minuteBarResult = BmllStatusMinuteBarResult.InProgress;

            while (!hasSuccess && !cts.Token.IsCancellationRequested)
            {
                _logger.LogInformation($"BmllDataRequestSenderManager BlockUntilBmllWorkIsDone in loop");

                var request = new GetMinuteBarRequestStatusesRequest { Keys = keys?.ToList() };

                Thread.Sleep(1000 * 15);
                minuteBarResult = await _timeBarRepository.StatusMinuteBars(request);

                hasSuccess =
                    minuteBarResult == BmllStatusMinuteBarResult.Completed
                    || minuteBarResult == BmllStatusMinuteBarResult.CompletedWithFailures;
            }

            if (cts.Token.IsCancellationRequested)
            {
                _logger.LogInformation($"BmllDataRequestSenderManager BlockUntilBmllWorkIsDone timed out after one hour.");
            }

            _logger.LogInformation($"BmllDataRequestSenderManager BlockUntilBmllWorkIsDone completed");

            return minuteBarResult;
        }

        private IReadOnlyCollection<IGetTimeBarPair> GetTimeBars(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            if (keys == null
                || !keys.Any())
            {
                _logger.LogError($"BmllDataRequestsManager received 0 data requests and did not have any to send on after projecting to GetMinuteBarsRequests");

                return new IGetTimeBarPair[0];
            }

            var minuteBarRequests = keys.Select(GetMinuteBarsRequest).Where(i => i != null).ToList();
            var consolidatedMinuteBarRequests = ConsolidatedMinuteBars(minuteBarRequests);

            if (!consolidatedMinuteBarRequests.Any())
            {
                _logger.LogError($"BmllDataRequestsManager received {keys.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

                return new IGetTimeBarPair[0];
            }

            var timeBarResponses = new List<IGetTimeBarPair>();

            foreach (var req in consolidatedMinuteBarRequests)
            {
                var responseTask = _timeBarRepository.GetMinuteBars(req);
                responseTask.Wait();
                var pair = new GetTimeBarPair(req, responseTask.Result);
                timeBarResponses.Add(pair);
            }

            return timeBarResponses;
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MinuteBarRequestKeyDto request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Figi))
            {
                _logger.LogError($"BmllDataRequestsSenderManager had a null request or a request that did not pass data request validation for {request?.Figi}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Figi))
            {
                _logger.LogError($"BmllDataRequestsSenderManager asked to process a security without a figi");

                return null;
            }

            return new GetMinuteBarsRequest
            {
                Figi = request.Figi,
                From = request.Date.Date,
                To = request.Date.Date.AddDays(1).AddMilliseconds(-1),
                Interval = "1min",
            };
        }

        private List<GetMinuteBarsRequest> ConsolidatedMinuteBars(List<GetMinuteBarsRequest> requests)
        {
            if (requests == null
                || !requests.Any())
            {
                return new List<GetMinuteBarsRequest>();
            }

            var result = new List<GetMinuteBarsRequest>();

            var groupedByFigi = requests.GroupBy(req => req.Figi);

            foreach (var grp in groupedByFigi)
            {
                var from = grp.Min(x => x.From);
                var to = grp.Max(x => x.To);
                var newRequest = new GetMinuteBarsRequest
                {
                    Figi = grp.FirstOrDefault()?.Figi,
                    From = from,
                    To = to,
                    Interval = grp.FirstOrDefault()?.Interval
                };

                result.Add(newRequest);
            }

            return result;
        }
    }
}
