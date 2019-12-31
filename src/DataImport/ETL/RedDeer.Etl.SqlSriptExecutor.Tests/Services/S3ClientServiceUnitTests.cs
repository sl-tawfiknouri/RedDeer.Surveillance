using Amazon.S3;
using Amazon.S3.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Tests.Services
{
    public class S3ClientServiceUnitTests
    {
        private S3ClientService _s3ClientService = null;

        private IAmazonS3 _amazonS3 = null;
        private IAmazonS3ClientFactory _amazonS3ClientFactory = null;

        [SetUp]
        public void SetUp()
        {
            _amazonS3ClientFactory = A.Fake<IAmazonS3ClientFactory>();
            _amazonS3 = A.Fake<IAmazonS3>();

            A.CallTo(() => _amazonS3ClientFactory.Create())
                .Returns(_amazonS3);

            _s3ClientService = new S3ClientService(_amazonS3ClientFactory, new NullLogger<S3ClientService>());
        }

        [Test]
        public async Task ReadAllText_WhenExecuted_ReturnsFileContent()
        {
            var respose = new GetObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("file content"))
            };

            A.CallTo(() => _amazonS3.GetObjectAsync(A<GetObjectRequest>.That.Matches(m => m.BucketName == "test-bucket" && m.Key == "location/file-key.sql"), CancellationToken.None))
                .Returns(Task.FromResult(respose));

            var fileContent = await _s3ClientService.ReadAllText("S3://test-bucket/location/file-key.sql");

            fileContent.Should().Be("file content");
        }
    }
}
