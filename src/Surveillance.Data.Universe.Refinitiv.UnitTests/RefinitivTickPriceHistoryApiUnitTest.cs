//using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.Options;
using NUnit.Framework;
//using RedDeer.Extensions.Security.Authentication.Jwt;
//using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
//using Google.Protobuf.WellKnownTypes;
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
            //var jwtTokenService = new JwtTokenService();

            //var config = new Dictionary<string, string>
            //{
            //    //    { "EC2Tags:Environment", "dev" },
            //    //    { "EC2Tags:Customer", "reddeer" }
            //    { "TickPriceHistoryServiceAddress", "localhost:8889" }
            //};

            //var configuration = new ConfigurationBuilder()
            //    .AddInMemoryCollection(config)
            //    .Build();

            //var options = new TickPriceHistoryServiceClientOptions
            //{
            //    Address = "localhost:8888", // "https://localhost:8889",
            //    JwtBearerTokenSymetricSecurityKey = "nfPA%sowa62L9U$DxWyqD2xXRZrBvH7iWBtdqhWu!U^1qTklZS"
            //};

            //var ioptions = Options.Create(options);
            var refinitivTickPriceHistoryApiConfig = new RefinitivTickPriceHistoryApiConfig
            {
                RefinitivTickPriceHistoryApiAddress = "localhost:8888"
            };

            var factory = new TickPriceHistoryServiceClientFactory(refinitivTickPriceHistoryApiConfig);

            //var factory = new TickPriceHistoryServiceClientFactory(configuration, jwtTokenService);
            var refinitivTickPriceHistoryApi = new RefinitivTickPriceHistoryApi(factory);
            
            var response = await refinitivTickPriceHistoryApi.GetInterdayTimeBars(DateTime.UtcNow.Date.AddDays(-2), DateTime.UtcNow.Date);
        }
    }
}