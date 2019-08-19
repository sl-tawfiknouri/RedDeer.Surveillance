namespace DataSynchroniser.Api.Bmll.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;

    using Firefly.Service.Data.BMLL.Shared.Requests;

    using Microsoft.Extensions.Logging;

    using PollyFacade.Policies.Interfaces;

    using SharedKernel.Contracts.Markets;

    public class BmllDataRequestsManager : IBmllDataRequestManager
    {
        private readonly IBmllDataRequestsApiManager _apiManager;

        private readonly ILogger<BmllDataRequestsManager> _logger;

        private readonly IPolicyFactory _policyFactory;

        private readonly IBmllDataRequestsStorageManager _storageManager;

        public BmllDataRequestsManager(
            IBmllDataRequestsApiManager apiManager,
            IBmllDataRequestsStorageManager storageManager,
            IPolicyFactory policyFactory,
            ILogger<BmllDataRequestsManager> logger)
        {
            this._apiManager = apiManager ?? throw new ArgumentNullException(nameof(apiManager));
            this._storageManager = storageManager ?? throw new ArgumentNullException(nameof(storageManager));
            this._policyFactory = policyFactory ?? throw new ArgumentNullException(nameof(policyFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static List<List<T>> SplitList<T>(List<T> bmllRequests, int splitSize)
        {
            if (bmllRequests == null || !bmllRequests.Any()) return new List<List<T>>();

            var result = new List<List<T>>();

            var totalIterations = bmllRequests.Count / splitSize + 1;

            for (var x = 1; x <= totalIterations; x++)
            {
                var slice = bmllRequests.Skip((x - 1) * splitSize).Take(splitSize).ToList();
                result.Add(slice);
            }

            return result;
        }

        public async Task Submit(string systemOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests)
        {
            var filteredBmllRequests = bmllRequests.Where(req => !req?.IsCompleted ?? false).ToList();

            var splitLists = SplitList(filteredBmllRequests, 400); // more reliable but slower with a smaller increment
            var splitTasks = SplitList(splitLists, 4);

            foreach (var splitTask in splitTasks)
            {
                var split = splitTask.Select(this.ProcessBmllRequests).ToList();
                Task.WhenAll(split).Wait(TimeSpan.FromMinutes(20));
            }

            await Task.CompletedTask;
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MarketDataRequest request)
        {
            if (request == null || (!request?.IsValid() ?? true))
            {
                this._logger.LogError(
                    $"{nameof(BmllDataRequestsManager)} had a null request or a request that did not pass data request validation for {request?.Identifiers}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Identifiers.Figi))
            {
                this._logger.LogError($"{nameof(BmllDataRequestsManager)} asked to process a security without a figi");

                return null;
            }

            return new GetMinuteBarsRequest
                       {
                           Figi = request.Identifiers.Figi,
                           From = request.UniverseEventTimeFrom.Value.Date,
                           To = request.UniverseEventTimeTo.Value.Date.AddDays(1).AddMilliseconds(-1),
                           Interval = "1min"
                       };
        }

        private async Task ProcessBmllRequests(List<MarketDataRequest> bmllRequests)
        {
            this._logger.LogInformation(
                $"{nameof(BmllDataRequestsManager)} received {bmllRequests.Count} data requests");

            var minuteBarRequests = bmllRequests.Select(this.GetMinuteBarsRequest).Where(i => i != null).ToList();

            if (!minuteBarRequests.Any())
            {
                this._logger.LogError(
                    $"{nameof(BmllDataRequestsManager)} received {bmllRequests.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

                return;
            }

            SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>> requestResult = null;
            var policyWrap =
                this._policyFactory.PolicyTimeoutGeneric<SuccessOrFailureResult<IReadOnlyCollection<IGetTimeBarPair>>>(
                    TimeSpan.FromMinutes(15),
                    i => !i.Success,
                    3,
                    TimeSpan.FromSeconds(30));

            await policyWrap.ExecuteAsync(
                async () =>
                    {
                        requestResult = await this._apiManager.Send(bmllRequests, false);

                        return requestResult;
                    });

            if (!requestResult?.Success ?? true) requestResult = await this._apiManager.Send(bmllRequests, true);

            await this._storageManager.Store(requestResult.Value);
        }
    }
}