using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;
using TestHarness.Configuration.Interfaces;
using TestHarness.Repository.Api.Interfaces;

namespace TestHarness.Repository.Api
{
    public class SecurityApiRepository : BaseApiRepository, ISecurityApiRepository
    {
        private const string HeartBeatUrl = "api/security/heartbeat";
        private const string MarketsUrl = "api/security/get/v1?from=frdate&to=tdate&market=mrk";

        public SecurityApiRepository(INetworkConfiguration networkConfiguration)
            : base(networkConfiguration)
        { }

        public async Task<bool> Heartbeating()
        {
            try
            {
                var client = BuildHttpClient();

                var response = await client.GetAsync(HeartBeatUrl);

                return (response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<SecurityPriceResponseDto> Get(DateTime from, DateTime to, string market)
        {
            var client = BuildHttpClient();

            var url =
                MarketsUrl
                    .Replace("frdate", from.ToString("MM/dd/yyyy"))
                    .Replace("tdate", to.ToString("MM/dd/yyyy"))
                    .Replace("mrk", market);

            var response = await client.GetAsync(url);
            var respJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SecurityPriceResponseDto>(respJson);
        }
    }
}
