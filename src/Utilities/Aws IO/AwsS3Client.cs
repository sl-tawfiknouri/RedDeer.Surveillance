using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Utilities.Aws_IO.Interfaces;

namespace Utilities.Aws_IO
{
    public class AwsS3Client : IAwsS3Client
    {
        private readonly AmazonS3Client _s3Client;
        private readonly ILogger<AwsS3Client> _logger;

        public AwsS3Client(ILogger<AwsS3Client> logger)
        {
            _s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> RetrieveFile(string bucketName, string key, string versionId, string targetFile, bool retry = true)
        {
            try
            {
                var cts = new CancellationTokenSource();

                var getObjectRequest = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key,
                    VersionId = versionId
                };

                _logger.LogInformation($"AwsS3Client fetching object with key {key}, versionId {versionId} from bucket {bucketName}.");
                var result = await _s3Client.GetObjectAsync(getObjectRequest, cts.Token);

                _logger.LogInformation($"AwsS3Client fetched object with key {key}, versionId {versionId} from bucket {bucketName}. Now writing to disk at {targetFile}");
                await result.WriteResponseStreamToFileAsync(targetFile, false, cts.Token);

                return true;
            }
            catch (Exception e)
            {
                if (retry)
                {
                    var newKey = Uri.UnescapeDataString(key).Replace('+', ' ');
                    return await RetrieveFile(bucketName, newKey, versionId, targetFile, false);
                }
                else
                {
                    _logger.LogCritical(e.Message);
                    return false;
                }
            }
        }
    }
}