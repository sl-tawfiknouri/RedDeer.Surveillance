using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Domain.Markets;
using Firefly.Service.Data.BMLL.Shared.Commands;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Polly;
using Surveillance.DataLayer.Api.BmllMarketData;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class BmllDataRequestsSenderManager : IBmllDataRequestsSenderManager
    {
        private readonly IBmllDataRequestsGetTimeBars _requestsGetTimeBars;
        private readonly IMarketDataRequestToMinuteBarRequestKeyDtoProjector _marketDataRequestProjector;
        private readonly IBmllTimeBarApiRepository _timeBarRepository;
        private readonly ILogger<BmllDataRequestsSenderManager> _logger;

        public BmllDataRequestsSenderManager(
            IBmllDataRequestsGetTimeBars requestsGetTimeBars,
            IMarketDataRequestToMinuteBarRequestKeyDtoProjector marketDataRequestProjector,
            IBmllTimeBarApiRepository timeBarRepository,
            ILogger<BmllDataRequestsSenderManager> logger)
        {
            _requestsGetTimeBars = requestsGetTimeBars ?? throw new ArgumentNullException(nameof(requestsGetTimeBars));
            _marketDataRequestProjector = marketDataRequestProjector ?? throw new ArgumentNullException(nameof(marketDataRequestProjector));
            _timeBarRepository = timeBarRepository ?? throw new ArgumentNullException(nameof(timeBarRepository)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(
            List<MarketDataRequest> bmllRequests,
            bool completeWithFailures)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} received a null or empty requests collection. Returning.");
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            var result = await BlockOnHeartbeatDown();
            if (!result)
            {
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            try
            {
                _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} beginning 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars");

                // step 0.
                // project to request keys
                var keys = _marketDataRequestProjector.ProjectToRequestKeys(bmllRequests);

                if (keys == null
                    || !keys.Any())
                {
                    _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars) at Project Keys. Had no keys to fetch. Returning.");

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
                var timeBarResponses = _requestsGetTimeBars.GetTimeBars(keys);

                _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars)");

                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, timeBarResponses);
            }
            catch (Exception e)
            {
                _logger?.LogError($"{nameof(BmllDataRequestsSenderManager)} encountered an unexpected error during processing. {e.Message} {e.InnerException?.Message}");
            }

            return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
        }

        private async Task<bool> BlockOnHeartbeatDown()
        {
            try
            {
                var cts = new CancellationTokenSource(1000 * 60 * 15);
                var checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);

                while (!checkHeartbeat)
                {
                    if (cts.IsCancellationRequested)
                    {
                        _logger.LogError($"{nameof(BmllDataRequestsSenderManager)} ran out of time to connect to the API. Returning an empty response.");
                        return false;
                    }

                    _logger.LogError($"{nameof(BmllDataRequestsSenderManager)} could not elicit a successful heartbeat response. Waiting for a maximum of 30 minutes...");
                    Thread.Sleep(1000 * 30);
                    checkHeartbeat = await _timeBarRepository.HeartBeating(cts.Token);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError($"{nameof(BmllDataRequestsSenderManager)} Send encountered an error whilst monitoring for heartbeating...", e);
                return false;
            }
        }

        private async Task CreateMinuteBarRequest(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} CreateMinuteBarRequest active");

            var request = new CreateMinuteBarRequestCommand { Keys = keys?.ToList() };

            await _timeBarRepository.RequestMinuteBars(request);

            _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} CreateMinuteBarRequest complete");
        }

        private async Task<BmllStatusMinuteBarResult> BlockUntilBmllWorkIsDone(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} BlockUntilBmllWorkIsDone active");

            var timeoutPolicy = Policy.TimeoutAsync<BmllStatusMinuteBarResult>(TimeSpan.FromMinutes(20));
            var retryPolicy =
                Policy
                    .Handle<Exception>()
                    .OrResult<BmllStatusMinuteBarResult>(i => i == BmllStatusMinuteBarResult.InProgress)
                    .WaitAndRetryAsync(15, i => TimeSpan.FromMinutes(1));

            var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

            var minuteBarResult = BmllStatusMinuteBarResult.InProgress;
            var request = new GetMinuteBarRequestStatusesRequest { Keys = keys?.ToList() };

            await policyWrap.ExecuteAsync(async () =>
            {
                minuteBarResult = await _timeBarRepository.StatusMinuteBars(request);

                return minuteBarResult;
            });
            
            _logger.LogInformation($"{nameof(BmllDataRequestsSenderManager)} BlockUntilBmllWorkIsDone completed");

            return minuteBarResult;
        }
    }
}
