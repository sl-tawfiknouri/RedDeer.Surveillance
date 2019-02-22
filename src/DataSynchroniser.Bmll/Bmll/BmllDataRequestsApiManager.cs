using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Domain.Markets;
using Firefly.Service.Data.BMLL.Shared.Commands;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using PollyFacade.Policies.Interfaces;
using Surveillance.DataLayer.Api.BmllMarketData;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class BmllDataRequestsApiManager : IBmllDataRequestsApiManager
    {
        private readonly IBmllDataRequestsGetTimeBars _requestsGetTimeBars;
        private readonly IMarketDataRequestToMinuteBarRequestKeyDtoProjector _marketDataRequestProjector;
        private readonly IPolicyFactory _policyFactory;
        private readonly IBmllTimeBarApiRepository _timeBarRepository;
        private readonly ILogger<BmllDataRequestsApiManager> _logger;

        public BmllDataRequestsApiManager(
            IBmllDataRequestsGetTimeBars requestsGetTimeBars,
            IMarketDataRequestToMinuteBarRequestKeyDtoProjector marketDataRequestProjector,
            IPolicyFactory policyFactory,
            IBmllTimeBarApiRepository timeBarRepository,
            ILogger<BmllDataRequestsApiManager> logger)
        {
            _requestsGetTimeBars = requestsGetTimeBars ?? throw new ArgumentNullException(nameof(requestsGetTimeBars));
            _marketDataRequestProjector = marketDataRequestProjector ?? throw new ArgumentNullException(nameof(marketDataRequestProjector));
            _policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
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
                _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} received a null or empty requests collection. Returning.");
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} beginning 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars");

            // step 0.
            // project to request keys
            var keys = _marketDataRequestProjector.ProjectToRequestKeys(bmllRequests);

            if (keys == null
                || !keys.Any())
            {
                _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars) at Project Keys. Had no keys to fetch. Returning.");

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

            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars)");

            return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, timeBarResponses);
        }

        private async Task CreateMinuteBarRequest(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} CreateMinuteBarRequest active");

            var request = new CreateMinuteBarRequestCommand { Keys = keys?.ToList() };

            await _timeBarRepository.RequestMinuteBars(request);

            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} CreateMinuteBarRequest complete");
        }

        private async Task<BmllStatusMinuteBarResult> BlockUntilBmllWorkIsDone(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} BlockUntilBmllWorkIsDone active");

            var policyWrap =
                _policyFactory.PolicyTimeoutGeneric<BmllStatusMinuteBarResult>(
                    TimeSpan.FromMinutes(20),
                    i => i == BmllStatusMinuteBarResult.InProgress,
                    15,
                    TimeSpan.FromMinutes(1));

            var minuteBarResult = BmllStatusMinuteBarResult.InProgress;
            var request = new GetMinuteBarRequestStatusesRequest { Keys = keys?.ToList() };

            await policyWrap.ExecuteAsync(async () =>
            {
                minuteBarResult = await _timeBarRepository.StatusMinuteBars(request);

                return minuteBarResult;
            });
            
            _logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} BlockUntilBmllWorkIsDone completed");

            return minuteBarResult;
        }
    }
}
