namespace TestHarness.Repository.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using TestHarness.Configuration.Interfaces;
    using TestHarness.Repository.Api.Interfaces;

    public class MarketApiRepository : BaseApiRepository, IMarketApiRepository
    {
        private const string HeartbeatRoute = "api/markets/heartbeat";

        private const string Route = "api/markets/get/v1";

        public MarketApiRepository(INetworkConfiguration networkConfiguration)
            : base(networkConfiguration)
        {
        }

        public async Task<IReadOnlyCollection<ExchangeDto>> Get()
        {
            var client = this.BuildHttpClient();

            var result = await client.GetAsync(Route);
            var marketString = await result.Content.ReadAsStringAsync();
            var marketData = JsonConvert.DeserializeObject<ExchangeDto[]>(marketString);

            return marketData;
        }

        public async Task<bool> HeartBeating()
        {
            var client = this.BuildHttpClient();

            var result = await client.GetAsync(HeartbeatRoute);

            return result.IsSuccessStatusCode;
        }
    }
}