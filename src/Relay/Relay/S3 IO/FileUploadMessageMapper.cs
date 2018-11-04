using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Relay.S3_IO.Interfaces;

namespace Relay.S3_IO
{
    public class FileUploadMessageMapper : IFileUploadMessageMapper
    {
        public FileUploadMessageDto Map(string json)
        {
            var converter = new ExpandoObjectConverter();

            dynamic message = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            dynamic message2 = JsonConvert.DeserializeObject<ExpandoObject>(message.Message, converter);

            return new FileUploadMessageDto
            {
                FileName = message2.Records[0].s3.@object.key,
                FileSize = message2.Records[0].s3.@object.size,
                Bucket = message2.Records[0].s3.bucket.name
            };
        }
    }
}
