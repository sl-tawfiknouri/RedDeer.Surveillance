using Amazon.S3.Util;
using System;

namespace RedDeer.Etl.SqlSriptExecutor.Extensions
{
    // https://docs.aws.amazon.com/AmazonS3/latest/dev/UsingBucket.html
    // https://docs.aws.amazon.com/AmazonS3/latest/dev/notification-content-structure.html
    public static class AmazonS3UriExtensions
    {
        public static bool TryParseAmazonS3Uri(string uriString, out AmazonS3Uri amazonS3Uri)
        {
            try
            {
                if (AmazonS3Uri.TryParseAmazonS3Uri(uriString, out amazonS3Uri))
                {
                    return true;
                }

                // "s3://bucket/key.txt"
                var uri = new Uri(uriString);
                amazonS3Uri = CreateAmazonS3Uri(uri.Host, uri.PathAndQuery);
                return true;
            }
            catch (Exception)
            {
                amazonS3Uri = null;
                return false;
            }
        }

        private static AmazonS3Uri CreateAmazonS3Uri(string bucketName, string objectKey)
            => new AmazonS3Uri(CreateS3Uri(bucketName, objectKey));

        public static Uri CreateS3Uri(string bucketName, string objectKey)
            => new Uri($"https://s3.amazonaws.com/{bucketName}/{objectKey?.Replace("+", " ")}");
    }
}
