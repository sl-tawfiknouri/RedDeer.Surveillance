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
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
    [assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace NeptuneAimOmsFileLambda
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
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            LambdaLogger.Log($"FUNCTION HANDLER FOR NEPTUNE AIM OMS FILE LAMBDA INVOKED InvokedFunctionARN - {context.InvokedFunctionArn}");
            LambdaLogger.Log($"FUNCTION INVOKED FOR EVENT {evnt.Records.FirstOrDefault().EventName} {evnt.Records.FirstOrDefault().EventSource}");
            var neptuneAimOmsDirectory = Environment.GetEnvironmentVariable("NeptuneAimOmsWriteDirectory");

            if (string.IsNullOrWhiteSpace(neptuneAimOmsDirectory))
            {
                LambdaLogger.Log($"DID NOT RECOGNISE {neptuneAimOmsDirectory}");
                throw new ArgumentOutOfRangeException(nameof(neptuneAimOmsDirectory));
            }

            var s3Event = evnt.Records?[0].S3;
            if(s3Event == null)
            {
                LambdaLogger.Log($"S3 EVENT WAS NULL");
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
                LambdaLogger.Log($"S3 EVENT WAS WRONG TYPE {evnt.Records.FirstOrDefault().EventName.Value}");
                return null;
            }

            try
            {
                LambdaLogger.Log($"S3 EVENT GET OBJECT REQUEST BUILT {s3Event.Bucket.Name} TARGET");
                var getObjectRequest = new GetObjectRequest() { BucketName = s3Event.Bucket.Name, Key = s3Event.Object.Key };
                LambdaLogger.Log($"S3 EVENT GET OBJECT REQUEST START {s3Event.Bucket.Name} TARGET");
                var s3Object = await this.S3Client.GetObjectAsync(getObjectRequest);
                LambdaLogger.Log($"S3 EVENT GET OBJECT REQUEST END {s3Event.Bucket.Name} TARGET");

                var memStream = new MemoryStream();
                using (var str = s3Object.ResponseStream)
                {
                    LambdaLogger.Log($"S3 EVENT GET OBJECT REQUEST COPY TO MEM STREAM");
                    await str.CopyToAsync(memStream);
                }

                LambdaLogger.Log($"S3 EVENT GET OBJECT REQUEST COPY TO MEM STREAM COMPLETE {memStream.Length} length");
                memStream.Position = 0;
                var sr = new StreamReader(memStream);
                LambdaLogger.Log($"{memStream.Position} position");
                var redData = sr.ReadLine();
                LambdaLogger.Log($"{memStream.Position} position - read {redData}");
                var readData = sr.ReadLine();
                LambdaLogger.Log($"{memStream.Position} position - read {readData}");
                var clientData = await sr.ReadToEndAsync();
                LambdaLogger.Log($"{memStream.Position} position");

                var wStream = new MemoryStream();
                var swriter = new StreamWriter(wStream);
                swriter.Write(clientData);

                LambdaLogger.Log($"S3 READ TWO LINES FROM MEM STREAM VIA STREAM READER");
                var putObjectRequest = new PutObjectRequest()
                {
                    AutoCloseStream = true,
                    AutoResetStreamPosition = true,
                    BucketName = neptuneAimOmsDirectory,
                    Key = $"trimmed-header-{s3Event.Object.Key}",
                    InputStream = wStream,
                };

                LambdaLogger.Log($"S3 WRITING PUT OBJECT REQUEST");
                await this.S3Client.PutObjectAsync(putObjectRequest);
                LambdaLogger.Log($"S3 FINISHED WRITING PUT OBJECT REQUEST");

                return null;
            }
            catch(Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }
    }
}
