using System.Collections.Generic;
using System.Linq;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class MarketDataRequestToMinuteBarRequestKeyDtoProjector : IMarketDataRequestToMinuteBarRequestKeyDtoProjector
    {
        public IReadOnlyCollection<MinuteBarRequestKeyDto> ProjectToRequestKeys(IList<MarketDataRequest> bmllRequests)
        {
            if (bmllRequests == null
                || !bmllRequests.Any())
            {
                return new List<MinuteBarRequestKeyDto>();
            }

            var projectKeys = Project(bmllRequests);
            var filteredKeys = projectKeys.Where(key => !string.IsNullOrWhiteSpace(key.Figi)).ToList();
            var deduplicatedKeys = DeduplicateKeys(filteredKeys);

            return deduplicatedKeys;
        }

        private IList<MinuteBarRequestKeyDto> Project(IList<MarketDataRequest> bmllRequests)
        {
            var keys = new List<MinuteBarRequestKeyDto>();

            foreach (var req in bmllRequests)
            {
                if (req == null
                    || string.IsNullOrWhiteSpace(req.Identifiers.Figi)
                    || req.UniverseEventTimeTo == null
                    || req.UniverseEventTimeFrom == null)
                {
                    continue;
                }

                var toTarget = req.UniverseEventTimeTo.Value;
                var fromTarget = req.UniverseEventTimeFrom.Value;

                var timeSpan = toTarget.Subtract(fromTarget);
                var totalDays = timeSpan.TotalDays + 1;
                var iter = 0;

                while (iter <= totalDays)
                {
                    var date = fromTarget.AddDays(iter);

                    var barRequest = new MinuteBarRequestKeyDto(req.Identifiers.Figi, "1min", date);
                    keys.Add(barRequest);

                    iter += 1;
                }
            }

            return keys;
        }

        private List<MinuteBarRequestKeyDto> DeduplicateKeys(IList<MinuteBarRequestKeyDto> filteredKeys)
        {
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
    }
}
