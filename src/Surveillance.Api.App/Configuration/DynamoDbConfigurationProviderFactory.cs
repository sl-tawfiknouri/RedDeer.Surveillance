namespace Surveillance.Api.App.Configuration
{
    using System.Net;

    using Amazon;
    using Amazon.DynamoDBv2;

    using NLog;

    public static class DynamoDbConfigurationProviderFactory
    {
        private static DynamoDbConfigurationProvider _dynamoDbConfigurationProvider;

        public static DynamoDbConfigurationProvider Create()
        {
            if (_dynamoDbConfigurationProvider == null)
                _dynamoDbConfigurationProvider = GetDynamoDbConfigurationProvider();

            return _dynamoDbConfigurationProvider;
        }

        private static DynamoDbConfigurationProvider GetDynamoDbConfigurationProvider()
        {
            var client = new AmazonDynamoDBClient(
                new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var environmentService = new EnvironmentService();
            var logger = LogManager.GetLogger(nameof(DynamoDbConfigurationProvider));

            var config = new DynamoDbConfigurationProvider(environmentService, client, logger);
            return config;
        }
    }
}