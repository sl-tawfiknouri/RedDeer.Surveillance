using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Domain.Markets;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class BmllDataRequestsManager : IBmllDataRequestManager
    {
        private readonly IBmllDataRequestsApiManager _apiManager;
        private readonly IBmllDataRequestsStorageManager _storageManager;
        private readonly ILogger<BmllDataRequestsManager> _logger;

        public BmllDataRequestsManager(
            IBmllDataRequestsApiManager apiManager,
            IBmllDataRequestsStorageManager storageManager,
            ILogger<BmllDataRequestsManager> logger)
        {
            _apiManager = apiManager ?? throw new ArgumentNullException(nameof(apiManager));
            _storageManager = storageManager ?? throw new ArgumentNullException(nameof(storageManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Submit(string systemOperationId, IReadOnlyCollection<MarketDataRequest> bmllRequests)
        {
            var filteredBmllRequests = bmllRequests.Where(req => !req?.IsCompleted ?? false).ToList();

            var splitLists = SplitList(filteredBmllRequests, 400); // more reliable but slower with a smaller increment
            var splitTasks = SplitList(splitLists, 4);

            foreach (var splitTask in splitTasks)
            {
                var split = splitTask.Select(ProcessBmllRequests).ToList();
                Task.WhenAll(split).Wait(TimeSpan.FromMinutes(20));
            }
        }

        private async Task ProcessBmllRequests(List<MarketDataRequest> bmllRequests)
        {
            try
            {
                _logger.LogInformation($"{nameof(BmllDataRequestsManager)} received {bmllRequests.Count} data requests");

                var minuteBarRequests = bmllRequests.Select(GetMinuteBarsRequest).Where(i => i != null).ToList();

                if (!minuteBarRequests.Any())
                {
                    _logger.LogError(
                        $"{nameof(BmllDataRequestsManager)} received {bmllRequests.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

                    return;
                }

                // REQUEST IT
                var requests = await _apiManager.Send(bmllRequests, false);
                var retries = 3;

                while ((!requests.Success) && retries > 0)
                {
                    _logger.LogWarning($"{nameof(BmllDataRequestsManager)} received {bmllRequests.Count} data requests but had some failed requests. Retrying loop {retries}");

                    var forceCompletion = retries == 1;
                    requests = await _apiManager.Send(bmllRequests, forceCompletion);

                    retries -= 1;
                }

                // STORE IT
                await _storageManager.Store(requests.Value);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(BmllDataRequestsManager)} Send encountered an exception!", e);
            }
        }

        private GetMinuteBarsRequest GetMinuteBarsRequest(MarketDataRequest request)
        {
            if (request == null
                || (!request?.IsValid() ?? true))
            {
                _logger.LogError($"{nameof(BmllDataRequestsManager)} had a null request or a request that did not pass data request validation for {request?.Identifiers}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Identifiers.Figi))
            {
                _logger.LogError($"{nameof(BmllDataRequestsManager)} asked to process a security without a figi");

                return null;
            }

            return new GetMinuteBarsRequest
            {
                Figi = request.Identifiers.Figi,
                From = request.UniverseEventTimeFrom.Value.Date,
                To = request.UniverseEventTimeTo.Value.Date.AddDays(1).AddMilliseconds(-1),
                Interval = "1min",
            };
        }

        public static List<List<T>> SplitList<T>(List<T> bmllRequests, int splitSize)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                return new List<List<T>>();
            }

            var result = new List<List<T>>();

            var totalIterations = (bmllRequests.Count / splitSize) + 1;

            for (var x = 1; x <= totalIterations; x++)
            {
                var slice = bmllRequests.Skip((x - 1) * splitSize).Take(splitSize).ToList();
                result.Add(slice);
            }

            return result;
        }
    }
}
