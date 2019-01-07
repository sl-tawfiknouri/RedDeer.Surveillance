using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Manager.Bmll
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
            _logger.LogInformation($"BmllDataRequestsStorageManager beginning storage process for BMLL response data");

            if (timeBarPairs == null
                || !timeBarPairs.Any())
            {
                _logger.LogInformation($"BmllDataRequestsStorageManager completed storage process for BMLL response data as it had nothing to store.");
                return;
            }

            var selectMany = timeBarPairs.SelectMany(x => x.Response.MinuteBars).ToList();
            await _repository.Save(selectMany);
            _logger.LogInformation($"BmllDataRequestsStorageManager completed storage process for BMLL response data");
        }
    }
}
