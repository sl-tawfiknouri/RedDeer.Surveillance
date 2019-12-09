﻿using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using RedDeer.Extensions.Security.Authentication.Jwt;
using RedDeer.Extensions.Security.Authentication.Jwt.Abstractions;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using System;
using System.Threading.Tasks;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class TickPriceHistoryServiceClientFactory : ITickPriceHistoryServiceClientFactory
    {
        private readonly IRefinitivTickPriceHistoryApiConfig refinitivTickPriceHistoryApiConfig;
        private readonly IConfiguration configuration;
        private readonly IJwtTokenService jwtTokenService;

        public TickPriceHistoryServiceClientFactory(
            IRefinitivTickPriceHistoryApiConfig refinitivTickPriceHistoryApiConfig,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService)
        {
            this.refinitivTickPriceHistoryApiConfig = refinitivTickPriceHistoryApiConfig;
            this.configuration = configuration;
            this.jwtTokenService = jwtTokenService;
        }

        public TickPriceHistoryService.TickPriceHistoryServiceClient Create()
        {
            var address = refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiAddress;
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException($"Address '{address}' is null or empty.", nameof(refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiAddress));
            }

            var channel = string.IsNullOrWhiteSpace(refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey)
                ? CreateInsecureChannel()
                : CreateSecureChannel();
            
            var client = new TickPriceHistoryService.TickPriceHistoryServiceClient(channel);
            return client;
        }

        private ChannelBase CreateInsecureChannel()
            => new Channel(refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiAddress, ChannelCredentials.Insecure);

        private ChannelBase CreateSecureChannel()
        {
            var environmentTag = configuration["EC2TagsEnvironment"] ;
            var customerTag = configuration["EC2TagsCustomer"];
            var jwtBearerTokenSymetricSecurityKey = refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey;

            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                var jwtToken = new JwtToken()
                    .SetExpires(DateTime.UtcNow.AddMinutes(2))
                    .SetIssuer(environmentTag, customerTag, PermissionScopeTypeConstants.TickPriceHistory)
                    .SetAudience(environmentTag, customerTag, PermissionScopeTypeConstants.TickPriceHistory);

                var bearerToken = jwtTokenService.Generate(jwtToken, jwtBearerTokenSymetricSecurityKey);

                metadata.Add("Authorization", $"Bearer {bearerToken}");
                return Task.CompletedTask;
            });


            var channelCredentials = ChannelCredentials.Create(new SslCredentials(), credentials);
            var grpcChannelOptions = new GrpcChannelOptions
            {
                //HttpClient = httpClient,
                // LoggerFactory = logFactory,
                Credentials = channelCredentials
            };

            var channel = GrpcChannel.ForAddress(refinitivTickPriceHistoryApiConfig.RefinitivTickPriceHistoryApiAddress, grpcChannelOptions);
            return channel;
        }
    }
}
