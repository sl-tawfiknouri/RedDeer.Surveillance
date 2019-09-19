namespace DataSynchroniser.Api.Bmll.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;

    using Firefly.Service.Data.BMLL.Shared.Commands;
    using Firefly.Service.Data.BMLL.Shared.Dtos;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Reddeer.ApiClient.BmllMarketData;
    using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;

    public class BmllDataRequestsApiManager : IBmllDataRequestsApiManager
    {
        private readonly ILogger<BmllDataRequestsApiManager> _logger;

        private readonly IMarketDataRequestToMinuteBarRequestKeyDtoProjector _marketDataRequestProjector;

        private readonly IPolicyFactory _policyFactory;

        private readonly IBmllDataRequestsGetTimeBars _requestsGetTimeBars;

        private readonly IBmllTimeBarApi _timeBarRepository;

        public BmllDataRequestsApiManager(
            IBmllDataRequestsGetTimeBars requestsGetTimeBars,
            IMarketDataRequestToMinuteBarRequestKeyDtoProjector marketDataRequestProjector,
            IPolicyFactory policyFactory,
            IBmllTimeBarApi timeBarRepository,
            ILogger<BmllDataRequestsApiManager> logger)
        {
            this._requestsGetTimeBars =
                requestsGetTimeBars ?? throw new ArgumentNullException(nameof(requestsGetTimeBars));
            this._marketDataRequestProjector = marketDataRequestProjector
                                               ?? throw new ArgumentNullException(nameof(marketDataRequestProjector));
            this._policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this._timeBarRepository = timeBarRepository ?? throw new ArgumentNullException(nameof(timeBarRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>> Send(
            List<MarketDataRequest> bmllRequests,
            bool completeWithFailures)
        {
            if (bmllRequests == null || !bmllRequests.Any())
            {
                this._logger.LogInformation(
                    $"{nameof(BmllDataRequestsApiManager)} received a null or empty requests collection. Returning.");
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            this._logger.LogInformation(
                $"{nameof(BmllDataRequestsApiManager)} beginning 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars");

            // step 0.
            // project to request keys
            var keys = this._marketDataRequestProjector.ProjectToRequestKeys(bmllRequests);

            if (keys == null || !keys.Any())
            {
                this._logger.LogInformation(
                    $"{nameof(BmllDataRequestsApiManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars) at Project Keys. Had no keys to fetch. Returning.");

                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, new IGetTimeBarPair[0]);
            }

            // step 1.
            // create minute bar request
            await this.CreateMinuteBarRequest(keys);

            // step 2.
            // loop on the status update polling
            var bmllWorkResult = await this.BlockUntilBmllWorkIsDone(keys);

            if (bmllWorkResult == BmllStatusMinuteBarResult.CompletedWithFailures && !completeWithFailures)
                return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(false, new IGetTimeBarPair[0]);

            // step 3.
            // get minute bar request
            var timeBarResponses = this._requestsGetTimeBars.GetTimeBars(keys);

            this._logger.LogInformation(
                $"{nameof(BmllDataRequestsApiManager)} completed 4 step BMLL process (Project Keys; Create Minute Bars; Poll Minute Bars; Get MinuteBars)");

            return new SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>(true, timeBarResponses);
        }

        private async Task<BmllStatusMinuteBarResult> BlockUntilBmllWorkIsDone(
            IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            this._logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} BlockUntilBmllWorkIsDone active");

            var policyWrap = this._policyFactory.PolicyTimeoutGeneric<BmllStatusMinuteBarResult>(
                TimeSpan.FromMinutes(10),
                i => i == BmllStatusMinuteBarResult.InProgress,
                9,
                TimeSpan.FromMinutes(1));

            var minuteBarResult = BmllStatusMinuteBarResult.InProgress;
            var request = new GetMinuteBarRequestStatusesRequest { Keys = keys?.ToList() };

            await policyWrap.ExecuteAsync(
                async () =>
                    {
                        minuteBarResult = await this._timeBarRepository.StatusMinuteBarsAsync(request);

                        return minuteBarResult;
                    });

            this._logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} BlockUntilBmllWorkIsDone completed");

            return minuteBarResult;
        }

        private async Task CreateMinuteBarRequest(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            this._logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} CreateMinuteBarRequest active");

            var request = new CreateMinuteBarRequestCommand { Keys = keys?.ToList() };

            await this._timeBarRepository.RequestMinuteBarsAsync(request);

            this._logger.LogInformation($"{nameof(BmllDataRequestsApiManager)} CreateMinuteBarRequest complete");
        }
    }
}