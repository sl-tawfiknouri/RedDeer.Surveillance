using System;
using System.Collections.Generic;
using System.Linq;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Firefly.Service.Data.BMLL.Shared.Requests;
using Microsoft.Extensions.Logging;
using Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class BmllDataRequestsGetTimeBars : IBmllDataRequestsGetTimeBars
    {
        private readonly IBmllTimeBarApi _timeBarRepository;
        private readonly ILogger<BmllDataRequestsGetTimeBars> _logger;

        public BmllDataRequestsGetTimeBars(IBmllTimeBarApi timeBarRepository, ILogger<BmllDataRequestsGetTimeBars> logger)
        {
            _timeBarRepository = timeBarRepository ?? throw new ArgumentNullException(nameof(timeBarRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IGetTimeBarPair> GetTimeBars(IReadOnlyCollection<MinuteBarRequestKeyDto> keys)
        {
            if (keys == null
                || !keys.Any())
            {
                _logger.LogError($"{nameof(BmllDataRequestsGetTimeBars)} received 0 data requests and did not have any to send on after projecting to GetMinuteBarsRequests");

                return new IGetTimeBarPair[0];
            }

            var minuteBarRequests = keys.Select(GetMinuteBarsRequest).Where(i => i != null).ToList();
            var consolidatedMinuteBarRequests = ConsolidatedMinuteBars(minuteBarRequests);

            if (!consolidatedMinuteBarRequests.Any())
            {
                _logger.LogError($"{nameof(BmllDataRequestsGetTimeBars)} received {keys.Count} data requests but did not have any to send on after projecting to GetMinuteBarsRequests");

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
                _logger.LogError($"{nameof(BmllDataRequestsGetTimeBars)} had a null request or a request that did not pass data request validation for {request?.Figi}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Figi))
            {
                _logger.LogError($"{nameof(BmllDataRequestsGetTimeBars)} asked to process a security without a figi");

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

        private IList<GetMinuteBarsRequest> ConsolidatedMinuteBars(IList<GetMinuteBarsRequest> requests)
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
