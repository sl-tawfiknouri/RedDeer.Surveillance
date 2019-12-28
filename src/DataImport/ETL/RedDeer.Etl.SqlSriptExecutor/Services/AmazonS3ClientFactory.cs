using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System.Net;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class AmazonS3ClientFactory 
        : IAmazonS3ClientFactory
    {
        private readonly IEC2InstanceMetadataProvider _ec2InstanceMetadataProvider;
        private readonly ILogger<AmazonS3ClientFactory> _logger;

        public AmazonS3ClientFactory(
            IEC2InstanceMetadataProvider ec2InstanceMetadataProvider, 
            ILogger<AmazonS3ClientFactory> logger)
        {
            _ec2InstanceMetadataProvider = ec2InstanceMetadataProvider;
            _logger = logger;
        }

        public IAmazonS3 Create()
        {
            return new AmazonS3Client();
            //if (_ec2InstanceMetadataProvider.InstanceId() != null)
            //{
            //    return new AmazonS3Client();
            //}

            //var amazonEC2Config = new AmazonS3Config
            //{
            //    RegionEndpoint = RegionEndpoint.EUWest1,
            //    ProxyCredentials = CredentialCache.DefaultCredentials
            //};

            //return new AmazonS3Client(amazonEC2Config);
        }
    }
}
