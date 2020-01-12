using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Extensions;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class S3ClientService 
        : IS3ClientService
    {
        private readonly IAmazonS3ClientFactory _amazonS3ClientFactory;
        private readonly ILogger<S3ClientService> _logger;

        public S3ClientService(
            IAmazonS3ClientFactory amazonS3ClientFactory,
            ILogger<S3ClientService> logger)
        {
            _amazonS3ClientFactory = amazonS3ClientFactory;
            _logger = logger;
        }

        public async Task<string> ReadAllText(string uriString)
        {
            _logger.LogDebug($"Read all text for '{uriString}'.");

            var amazonS3Uri = AmazonS3UriExtensions.ParseAmazonS3Uri(uriString);

            var ms = await GetObjectStream(amazonS3Uri.Bucket, amazonS3Uri.Key, null);

            var bytes = ms.ToArray();
            var text = Encoding.UTF8.GetString(bytes);

            ms.Dispose();

            _logger.LogDebug($"Read all text for '{uriString}' done.");

            return text;
        }

        public async Task<MemoryStream> GetObjectStream(string bucket, string key, string versionId = null)
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucket,
                Key = key?.TrimStart('/'),
                VersionId = versionId
            };

            _logger.LogDebug($"Read object stream (Bucket: '{getObjectRequest.BucketName}', Key: '{getObjectRequest.Key}', VersionId: '{getObjectRequest.VersionId}').");

            var client = _amazonS3ClientFactory.Create();
            var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
            getObjectResponse.EnsureSuccessStatusCode();

            var ms = new MemoryStream();
            await getObjectResponse.ResponseStream.CopyToAsync(ms);

            _logger.LogDebug($"Read object stream done, total bytes: '{ms.Length}'. (Bucket: '{getObjectRequest.BucketName}', Key: '{getObjectRequest.Key}', VersionId: '{getObjectRequest.VersionId}').");

            return ms;
        }

        public async Task<bool> PutObjectStream(string bucket, string key, Stream stream)
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = stream,
                AutoResetStreamPosition = true
            };

            var streamLength = stream.Length;
            _logger.LogDebug($"Write object stream (Bucket: '{putObjectRequest.BucketName}', Key: '{putObjectRequest.Key}', Bytes: '{streamLength}').");

            var client = _amazonS3ClientFactory.Create();
            var getObjectResponse = await client.PutObjectAsync(putObjectRequest);
            getObjectResponse.EnsureSuccessStatusCode();

            _logger.LogDebug($"Write object stream done, total bytes: '{streamLength}'. (Bucket: '{putObjectRequest.BucketName}', Key: '{putObjectRequest.Key}').");

            return true;
        }

        public async Task<List<S3ObjectModel>> ListObjectsAsync(string uriString)
        {
            var amazonS3Uri = AmazonS3UriExtensions.ParseAmazonS3Uri(uriString);

            return await ListObjectsAsync(amazonS3Uri.Bucket, amazonS3Uri.Key);
        }

        public async Task<List<S3ObjectModel>> ListObjectsAsync(string bucket, string prefix)
        {
            var result = new List<S3ObjectModel>();

            var request = new ListObjectsRequest
            {
                BucketName = bucket,
                Prefix = prefix?.TrimStart('/'),
            };

            do
            {
                var response = await ListObjectsAsync(request);

                var s3ObjectModels = response.S3Objects.Select(entry => new S3ObjectModel
                {
                    BucketName = entry.BucketName,
                    Key = entry.Key,
                    LastModified = entry.LastModified,
                    Size = entry.Size
                });

                result.AddRange(s3ObjectModels);

                // If response is truncated, set the marker to get the next set of keys.
                if (response.IsTruncated)
                {
                    request.Marker = response.NextMarker;
                }
                else
                {
                    request = null;
                }
            }
            while (request != null);

            return result;
        }

        private async Task<ListObjectsResponse> ListObjectsAsync(ListObjectsRequest request)
        {
            var requestMsg = $"ListObjectsRequest (BucketName: '{request.BucketName}', Prefix: '{request.Prefix}, Marker: '{request.Marker}')";
            _logger.LogDebug(requestMsg);

            var client = _amazonS3ClientFactory.Create();
            var response = await client.ListObjectsAsync(request);
            response.EnsureSuccessStatusCode();

            _logger.LogDebug($"ListObjectsResponse (HttpStatus: '{response.HttpStatusCode}', ObjectsCount: '{response.S3Objects?.Count}') for {requestMsg}");
            return response;
        }
    }
}
