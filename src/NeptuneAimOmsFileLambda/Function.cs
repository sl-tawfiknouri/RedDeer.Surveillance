using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AimOmsFileHeaderTrimmerLambda
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }
        
        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        ///
        /// This was initially developed for neptune but is useful for any AIM OMS file that needs two header rows removed
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            LambdaLogger.Log($"Function handler for aim oms file lambda invoked. Invoked function arn - {context.InvokedFunctionArn}");
            LambdaLogger.Log($"Function invoked for event {evnt.Records.FirstOrDefault().EventName} {evnt.Records.FirstOrDefault().EventSource}");

            var aimOmsKey = Environment.GetEnvironmentVariable("AimOmsWriteKey");

            if (string.IsNullOrWhiteSpace(aimOmsKey))
            {
                LambdaLogger.Log($"Did not recognise {aimOmsKey}");
                throw new ArgumentOutOfRangeException(nameof(aimOmsKey));
            }

            aimOmsKey = aimOmsKey.Trim('/').Trim('\\');
            var splitKey = aimOmsKey.Split('/');

            var aimOmsKeyBucket = splitKey.First();
            var aimOmsKeyPath = splitKey.Skip(1).Aggregate((a, b) => a + '/' + b);

            var s3Event = evnt.Records?[0].S3;
            if(s3Event == null)
            {
                LambdaLogger.Log($"S3 event was null");
                return null;
            }

            var validEventTypes = new List<EventType>
            {
                EventType.ObjectCreatedAll,
                EventType.ObjectCreatedCompleteMultipartUpload,
                EventType.ObjectCreatedCopy,
                EventType.ObjectCreatedPost,
                EventType.ObjectCreatedPut
            };

            if (!validEventTypes.Contains(evnt.Records.FirstOrDefault()?.EventName))
            {
                LambdaLogger.Log($"S3 event was the wrong type {evnt.Records.FirstOrDefault().EventName.Value}");
                return null;
            }

            try
            {
                LambdaLogger.Log($"S3 event get object request built {s3Event.Bucket.Name} target");
                var getObjectRequest = new GetObjectRequest() { BucketName = s3Event.Bucket.Name, Key = s3Event.Object.Key };
                LambdaLogger.Log($"S3 event get object request start {s3Event.Bucket.Name} target");
                var s3Object = await this.S3Client.GetObjectAsync(getObjectRequest);
                LambdaLogger.Log($"S3 event get object request end {s3Event.Bucket.Name} target");

                var memStream = new MemoryStream();
                using (var str = s3Object.ResponseStream)
                {
                    LambdaLogger.Log($"S3 event get object copying bytes to memory stream");
                    await str.CopyToAsync(memStream);
                }

                LambdaLogger.Log($"S3 event get object response bytes written to a memory stream {memStream.Length} length");
                memStream.Position = 0;
                var sr = new StreamReader(memStream);
                LambdaLogger.Log($"{memStream.Position} position");
                var readDataLineOne = sr.ReadLine();
                LambdaLogger.Log($"{memStream.Position} position - read {readDataLineOne}");
                var readDataLineTwo = sr.ReadLine();
                LambdaLogger.Log($"{memStream.Position} position - read {readDataLineTwo}");
                var clientData = await sr.ReadToEndAsync();
                LambdaLogger.Log($"{memStream.Position} position");

                var wStream = new MemoryStream();
                var swriter = new StreamWriter(wStream);
                swriter.Write(clientData);

                var adjustedKey = s3Event.Object.Key.Split("/").LastOrDefault() ?? string.Empty;

                var putObjectRequest = new PutObjectRequest()
                {
                    AutoCloseStream = true,
                    AutoResetStreamPosition = true,
                    BucketName = aimOmsKeyBucket,
                    Key = $"{aimOmsKeyPath}/{adjustedKey}",
                    InputStream = wStream,
                };

                LambdaLogger.Log($"S3 about to send put object request.");
                var putObjectResponse = await this.S3Client.PutObjectAsync(putObjectRequest);
                LambdaLogger.Log($"S3 finished the put object request. Had status code {putObjectResponse.HttpStatusCode}");

                return null;
            }
            catch(Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function. Message {e.Message}. Stacktrace {e.StackTrace}");
                throw;
            }
        }
    }
}
