using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Firefly.Service.Data.BMLL.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace DataSynchroniser.Api.Bmll.Bmll
{
    public class BmllDataRequestsStorageManager : IBmllDataRequestsStorageManager
    {
        private readonly IReddeerMarketTimeBarRepository _repository;
        private readonly ILogger<BmllDataRequestsStorageManager> _logger;

        public BmllDataRequestsStorageManager(
            IReddeerMarketTimeBarRepository repository,
            ILogger<BmllDataRequestsStorageManager> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Store(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs)
        {
            _logger.LogInformation($"{nameof(BmllDataRequestsStorageManager)} beginning storage process for BMLL response data");

            if (timeBarPairs == null
                || !timeBarPairs.Any())
            {
                _logger.LogInformation($"{nameof(BmllDataRequestsStorageManager)} completed storage process for BMLL response data as it had nothing to store.");
                return;
            }

            var deduplicatedBars = Deduplicate(timeBarPairs);

            await _repository.Save(deduplicatedBars);

            _logger.LogInformation($"{nameof(BmllDataRequestsStorageManager)} completed storage process for BMLL response data");
        }

        private List<MinuteBarDto> Deduplicate(IReadOnlyCollection<IGetTimeBarPair> timeBarPairs)
        {
            var selectMany = timeBarPairs.SelectMany(x => x.Response.MinuteBars).Where(_ => _.ExecutionClosePrice != null).ToList();
            var figiGroups = selectMany.GroupBy(o => o.Figi);

            var deduplicatedBars = new List<MinuteBarDto>();
            foreach (var grp in figiGroups)
            {
                var firstByDateTime = grp.GroupBy(o => o.DateTime).Select(oo => oo.FirstOrDefault()).ToList();
                deduplicatedBars.AddRange(firstByDateTime);
            }

            return deduplicatedBars;
        }
    }
}
