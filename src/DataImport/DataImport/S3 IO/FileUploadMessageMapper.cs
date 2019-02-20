using System.Linq;
using Amazon.S3.Util;
using Amazon.SimpleNotificationService.Util;
using DataImport.S3_IO.Interfaces;
using Newtonsoft.Json;

namespace DataImport.S3_IO
{
    public class FileUploadMessageMapper : IFileUploadMessageMapper
    {
        public FileUploadMessageDto Map(string json)
        {
            var snsMessage = Message.ParseMessage(json);
            var s3Event = JsonConvert.DeserializeObject<S3EventNotification>(snsMessage.MessageText);
            var record = s3Event.Records.Single();

            return new FileUploadMessageDto
            {
                FileName = record.S3.Object.Key,
                FileSize = record.S3.Object.Size,
                VersionId = record.S3.Object.VersionId,
                Bucket = record.S3.Bucket.Name
            };
        }
    }
}
