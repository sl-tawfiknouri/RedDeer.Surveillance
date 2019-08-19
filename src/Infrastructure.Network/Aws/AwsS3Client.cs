namespace Infrastructure.Network.Aws
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    public class AwsS3Client : IAwsS3Client
    {
        private readonly ILogger<AwsS3Client> _logger;

        private readonly AmazonS3Client _s3Client;

        public AwsS3Client(ILogger<AwsS3Client> logger)
        {
            this._s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> RetrieveFile(
            string bucketName,
            string key,
            string versionId,
            string targetFile,
            bool retry = true)
        {
            try
            {
                var cts = new CancellationTokenSource();

                var getObjectRequest = new GetObjectRequest
                                           {
                                               BucketName = bucketName, Key = key, VersionId = versionId
                                           };

                this._logger.LogInformation(
                    $"fetching object with key {key}, versionId {versionId} from bucket {bucketName}.");
                var result = await this._s3Client.GetObjectAsync(getObjectRequest, cts.Token);

                this._logger.LogInformation(
                    $"fetched object with key {key}, versionId {versionId} from bucket {bucketName}. Now writing to disk at {targetFile}");
                await result.WriteResponseStreamToFileAsync(targetFile, false, cts.Token);

                return true;
            }
            catch (Exception e)
            {
                if (retry)
                {
                    var newKey = Uri.UnescapeDataString(key).Replace('+', ' ');
                    return await this.RetrieveFile(bucketName, newKey, versionId, targetFile, false);
                }

                this._logger.LogCritical(e.Message);
                return false;
            }
        }
    }
}