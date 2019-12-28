using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Extensions;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System;
using System.IO;
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
            if (!AmazonS3UriExtensions.TryParseAmazonS3Uri(uriString, out AmazonS3Uri amazonS3Uri))
            {
                throw new ArgumentException($"Not valid S3 uri '{uriString}'.", nameof(uriString));
            }

            var getObjectRequest = new GetObjectRequest
            {
                BucketName = amazonS3Uri.Bucket,
                Key = amazonS3Uri.Key.TrimStart('/'),
            };

            _logger.LogDebug($"Read all text for '{uriString}'. (Bucket: '{getObjectRequest.BucketName}', Key: '{getObjectRequest.Key}').");

            var client = _amazonS3ClientFactory.Create();
            var getObjectResponse = await client.GetObjectAsync(getObjectRequest);
            getObjectResponse.EnsureSuccessStatusCode();

            var ms = new MemoryStream();
            await getObjectResponse.ResponseStream.CopyToAsync(ms);

            var bytes = ms.ToArray();
            var text = Encoding.UTF8.GetString(bytes);

            ms.Dispose();

            _logger.LogDebug($"Read all text for '{uriString}' done.");

            return text;
        }
    }
}
