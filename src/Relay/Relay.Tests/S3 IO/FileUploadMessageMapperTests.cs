using NUnit.Framework;
using Relay.S3_IO;

namespace Relay.Tests.S3_IO
{
    [TestFixture]
    public class FileUploadMessageMapperTests
    {
        [Test]
        public void DoesDeserialiseBusMessage()
        {
            const string message = "{\r\n  \"Type\" : \"Notification\",\r\n  \"MessageId\" : \"3bbdaa79-b872-5668-b733-fe214a949fad\",\r\n  \"TopicArn\" : \"arn:aws:sns:eu-west-1:845656687399:dev-reddeer-ftp\",\r\n  \"Subject\" : \"Amazon S3 Notification\",\r\n  \"Message\" : \"{\\\"Records\\\":[{\\\"eventVersion\\\":\\\"2.0\\\",\\\"eventSource\\\":\\\"aws:s3\\\",\\\"awsRegion\\\":\\\"eu-west-1\\\",\\\"eventTime\\\":\\\"2018-11-04T13:14:14.615Z\\\",\\\"eventName\\\":\\\"ObjectCreated:Put\\\",\\\"userIdentity\\\":{\\\"principalId\\\":\\\"AWS:AROAJJRIFQ2OFO737ZVJG:Ryan\\\"},\\\"requestParameters\\\":{\\\"sourceIPAddress\\\":\\\"83.244.205.79\\\"},\\\"responseElements\\\":{\\\"x-amz-request-id\\\":\\\"AA3971C241BD3526\\\",\\\"x-amz-id-2\\\":\\\"+j+W5RbnGPZuxID0haKmbeVbnpneXZgFlDQlyUmltMCS2A51nzRWvxDR8yeuK7Dwnz8g43Wd+zg=\\\"},\\\"s3\\\":{\\\"s3SchemaVersion\\\":\\\"1.0\\\",\\\"configurationId\\\":\\\"ftp\\\",\\\"bucket\\\":{\\\"name\\\":\\\"reddeer-dev-client-old\\\",\\\"ownerIdentity\\\":{\\\"principalId\\\":\\\"A2XISU7RCIRGY4\\\"},\\\"arn\\\":\\\"arn:aws:s3:::reddeer-dev-client-old\\\"},\\\"object\\\":{\\\"key\\\":\\\"ftp/rdtest/rdtest/TradeFile-636769200847474564-426835f1-89d1-49bf-9307-43ddbdea0cf1.csv\\\",\\\"size\\\":2163351,\\\"eTag\\\":\\\"231eb329aad92bab873b45224a5e7978\\\",\\\"sequencer\\\":\\\"005BDEF0A67DB56DF8\\\"}}}]}\",\r\n  \"Timestamp\" : \"2018-11-04T13:14:14.709Z\",\r\n  \"SignatureVersion\" : \"1\",\r\n  \"Signature\" : \"ch0JkKKEFv1ADZ3Ov9HEzIwaY5fdMstiSdAGcFwgzNncmh8LfB4IwveID7IFYOfYUfcWw9i/1FtagfJgfianiTsQOiSrBU9zq49Zabp6u6CCYzFPHtLQVO2cEgMtTVoE0trb0NYkzRaKLUUZV2P+0NVVWszWHBJbjkjGIW+NkherkmWBFZYx+poapUlTzyi6oFcVEFj2ShBMtAdgsVDJAD0HDj8lOEPcg8knB6WhHqM1zO7iLiHJ5N9l4UIXxSQPDgLs7KS6Qh7syCUiPk7vJjXGE5adhbVWHB1dO0l9oPcsb/S+JGQG96TlAyP9NGpbMF+ZqQox2DqzeoLuq9mVcA==\",\r\n  \"SigningCertURL\" : \"https://sns.eu-west-1.amazonaws.com/SimpleNotificationService-ac565b8b1a6c5d002d285f9598aa1d9b.pem\",\r\n  \"UnsubscribeURL\" : \"https://sns.eu-west-1.amazonaws.com/?Action=Unsubscribe&SubscriptionArn=arn:aws:sns:eu-west-1:845656687399:dev-reddeer-ftp:ad51b5e2-99df-466a-960c-75741ff88116\"\r\n}";

            var mapper = new FileUploadMessageMapper();

            var dto = mapper.Map(message);

            Assert.IsNotNull(dto);
            Assert.AreEqual(dto.FileName, "ftp/rdtest/rdtest/TradeFile-636769200847474564-426835f1-89d1-49bf-9307-43ddbdea0cf1.csv");
            Assert.AreEqual(dto.FileSize, 2163351);
        }
    }
}
