namespace TestHarness.Repository.Api
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;

    using TestHarness.Configuration.Interfaces;
    using TestHarness.Repository.Api.Interfaces;

    public class SecurityApiRepository : BaseApiRepository, ISecurityApiRepository
    {
        private const string HeartBeatUrl = "api/security/heartbeat";

        private const string MarketsUrl = "api/security/get/v1?from=frdate&to=tdate&market=mrk";

        private readonly ILogger<SecurityApiRepository> logger;

        public SecurityApiRepository(INetworkConfiguration networkConfiguration, ILogger<SecurityApiRepository> logger)
            : base(networkConfiguration)
        {
            this.logger = logger;
        }

        public async Task<SecurityPriceResponseDto> Get(DateTime from, DateTime to, string market)
        {
            var client = this.BuildHttpClient();

            var url = MarketsUrl.Replace("frdate", from.ToString("MM/dd/yyyy"))
                .Replace("tdate", to.ToString("MM/dd/yyyy")).Replace("mrk", market);

            var response = await client.GetAsync(url);
            var respJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SecurityPriceResponseDto>(respJson);
        }

        public async Task<bool> Heartbeating()
        {
            try
            {
                var client = this.BuildHttpClient();

                var response = await client.GetAsync(HeartBeatUrl);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error while Heartbeating");

                return false;
            }
        }
    }
}