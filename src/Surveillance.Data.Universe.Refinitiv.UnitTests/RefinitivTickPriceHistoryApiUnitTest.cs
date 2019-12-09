using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RedDeer.Extensions.Security.Authentication.Jwt;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Surveillance.Data.Universe.Refinitiv.UnitTests
{
    public class RefinitivTickPriceHistoryApiUnitTest
    {
        [Test]
        [Ignore("ManualTest")]
        public async Task ManualTest()
        {
            var jwtTokenService = new JwtTokenService();

            var config = new Dictionary<string, string>
            {
                { "EC2Tags:Environment", "dev" },
                { "EC2Tags:Customer", "reddeer" },
                { "RefinitivTickPriceHistoryApiAddress", "https://localhost:8890" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            var refinitivTickPriceHistoryApiConfig = new RefinitivTickPriceHistoryApiConfig
            {
                RefinitivTickPriceHistoryApiAddress = "https://localhost:8890",
                RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey = "nfPA%sowa62L9U$DxWyqD2xXRZrBvH7iWBtdqhWu!U^1qTklZS"
            };

            var factory = new TickPriceHistoryServiceClientFactory(refinitivTickPriceHistoryApiConfig, configuration, jwtTokenService);
            var refinitivTickPriceHistoryApi = new RefinitivTickPriceHistoryApi(factory);
            
            var response = await refinitivTickPriceHistoryApi.GetInterdayTimeBars(DateTime.UtcNow.Date.AddDays(-2), DateTime.UtcNow.Date);
        }
    }
}