using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
//using RedDeer.Extensions.Security.Authentication.Jwt;
//using RedDeer.Extensions.Security.Authentication.Jwt.Abstractions;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using System;
//using System;
//using System.Threading.Tasks;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class TickPriceHistoryServiceClientOptions
    {
        public string Address { get; set; }

        public string JwtBearerTokenSymetricSecurityKey { get; set; }
    }

    public class TickPriceHistoryServiceClientFactory : ITickPriceHistoryServiceClientFactory
    {
        private readonly IRefinitivTickPriceHistoryApiConfig refinitivTickPriceHistoryApiConfig;
        // private readonly IConfiguration configuration;
        // private readonly IOptions<TickPriceHistoryServiceClientOptions> options;
        // private readonly IJwtTokenService jwtTokenService;

        public TickPriceHistoryServiceClientFactory(
            IRefinitivTickPriceHistoryApiConfig refinitivTickPriceHistoryApiConfig
            // IConfiguration configuration,
            // IOptions<TickPriceHistoryServiceClientOptions> options,
            // IJwtTokenService jwtTokenService
            )
        {
            this.refinitivTickPriceHistoryApiConfig = refinitivTickPriceHistoryApiConfig;
            //this.configuration = configuration;
            //this.options = options;
            //this.jwtTokenService = jwtTokenService;
        }

        public TickPriceHistoryService.TickPriceHistoryServiceClient Create()
        {
            //var address = configuration["TickPriceHistoryServiceAddress"];

            var address = refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiAddress;
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException($"Address '{address}' is null or empty.", "TickPriceHistoryServiceAddress");
            }

            var channel = new Channel(address, ChannelCredentials.Insecure);
            var client = new TickPriceHistoryService.TickPriceHistoryServiceClient(channel);
            return client;
        }

        /// <summary>
        /// Will be used later after service upgrade
        /// </summary>
        /// <returns></returns>
        private TickPriceHistoryService.TickPriceHistoryServiceClient CreateSecure()
        {
            //var environmentTag = configuration["EC2TagsEnvironment"];
            //var customerTag = configuration["EC2TagsCustomer"];
            //var jwtBearerTokenSymetricSecurityKey = options.Value.JwtBearerTokenSymetricSecurityKey;

            //var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            //{
            //    var jwtToken = new JwtToken()
            //        .SetExpires(DateTime.UtcNow.AddMinutes(2))
            //        .SetIssuer(environmentTag, customerTag, PermissionScopeTypeConstants.TickPriceHistory)
            //        .SetAudience(environmentTag, customerTag, PermissionScopeTypeConstants.TickPriceHistory);

            //    var bearerToken = jwtTokenService.Generate(jwtToken, jwtBearerTokenSymetricSecurityKey);

            //    metadata.Add("Authorization", $"Bearer {bearerToken}");
            //    return Task.CompletedTask;
            //});

            //var grpcChannelOptions = new GrpcChannelOptions
            //{
            //    HttpClient = httpClient,
            //    LoggerFactory = logFactory,
            //    Credentials = channelCredentials
            //};

            // var channel = GrpcChannel.ForAddress(options.Value.Address, grpcChannelOptions);
            //var channel = new Channel("localhost:8888", channelCredentials);
            var channel = new Channel("localhost:8888", ChannelCredentials.Insecure);
            // var channel = new Channel(options.Value.Address, ChannelCredentials.Insecure);
            var client = new TickPriceHistoryService.TickPriceHistoryServiceClient(channel);

            return client;
        }
    }
}
