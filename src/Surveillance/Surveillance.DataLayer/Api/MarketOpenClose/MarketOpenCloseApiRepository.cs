using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.MarketOpenClose
{
    public class MarketOpenCloseApiRepository : BaseApiRepository, IMarketOpenCloseApiRepository
    {
        private const string HeartbeatRoute = "api/markets/heartbeat";
        private const string Route = "api/markets/get/v1";
        private readonly ILogger _logger;

        public MarketOpenCloseApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<MarketOpenCloseApiRepository> logger) 
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            var httpClient = BuildHttpClient();

            try
            {
                var response = await httpClient.GetAsync(Route);

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful market open close api repository GET request. {response?.StatusCode}");

                    return new ExchangeDto[0];
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeDto[]>(jsonResponse);

                return deserialisedResponse ?? new ExchangeDto[0];
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new ExchangeDto[0];
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            var client = BuildHttpClient();

            var result = await client.GetAsync(HeartbeatRoute, token);

            return result.IsSuccessStatusCode;
        }
    }
}
