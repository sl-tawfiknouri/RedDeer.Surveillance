using System;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Factset.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace DataSynchroniser.Manager.Factset
{
    public class FactsetDataRequestsStorageManager : IFactsetDataRequestsStorageManager
    {
        private readonly IReddeerMarketDailySummaryRepository _repository;
        private readonly ILogger<FactsetDataRequestsStorageManager> _logger;

        public FactsetDataRequestsStorageManager(
            IReddeerMarketDailySummaryRepository repository,
            ILogger<FactsetDataRequestsStorageManager> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Store(FactsetSecurityResponseDto responseDto)
        {
            if (responseDto == null
                || responseDto.Responses == null
                || !responseDto.Responses.Any())
            {
                _logger?.LogInformation($"FactsetDataRequestsStorageManager Store received a null or empty factset daily summaries response");
                return;
            }

            _logger?.LogInformation($"FactsetDataRequestsStorageManager Store about to store the factset daily summary data");
            await _repository.Save(responseDto?.Responses);
            _logger?.LogInformation($"FactsetDataRequestsStorageManager Store has completed storing the factset daily summary data");
        }
    }
}
