using Amazon;
using Amazon.Athena;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System.Net;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class AmazonAthenaClientFactory : IAmazonAthenaClientFactory
    {
        private readonly IEC2InstanceMetadataProvider _ec2InstanceMetadataProvider;
        private readonly ILogger<AmazonAthenaClientFactory> _logger;

        public AmazonAthenaClientFactory(
            IEC2InstanceMetadataProvider ec2InstanceMetadataProvider,
            ILogger<AmazonAthenaClientFactory> logger)
        {
            _ec2InstanceMetadataProvider = ec2InstanceMetadataProvider;
            _logger = logger;
        }

        public IAmazonAthena Create()
        {
            if (_ec2InstanceMetadataProvider.InstanceId() != null)
            {
                return new AmazonAthenaClient();
            }

            var amazonEC2Config = new AmazonAthenaConfig
            {
                RegionEndpoint = RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            };

            return new AmazonAthenaClient(amazonEC2Config);
        }
    }
}
