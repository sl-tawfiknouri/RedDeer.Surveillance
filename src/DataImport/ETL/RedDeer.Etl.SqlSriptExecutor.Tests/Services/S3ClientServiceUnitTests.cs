using Amazon.S3;
using Amazon.S3.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                HttpStatusCode = HttpStatusCode.OK,
                ResponseStream = new MemoryStream(Encoding.UTF8.GetBytes("file content"))
            };

            A.CallTo(() => _amazonS3.GetObjectAsync(A<GetObjectRequest>.That.Matches(m => m.BucketName == "test-bucket" && m.Key == "location/file-key.sql"), CancellationToken.None))
                .Returns(Task.FromResult(respose));

            var fileContent = await _s3ClientService.ReadAllText("S3://test-bucket/location/file-key.sql");

            fileContent.Should().Be("file content");
        }

        [Test]
        public async Task PutObjectStream_WhenExecute_ShouldCallPutObjectAsync()
        {
            var respose = new PutObjectResponse
            {
                HttpStatusCode = HttpStatusCode.OK,
            };
            
            A.CallTo(() => _amazonS3.PutObjectAsync(A<PutObjectRequest>.That.Matches(m => m.BucketName == "test-bucket" && m.Key == "location/file-key.sql" && m.AutoResetStreamPosition == true), CancellationToken.None))
                .Returns(Task.FromResult(respose));
            
            var memoryStream = new MemoryStream();
            var result = await _s3ClientService.PutObjectStream("test-bucket", "location/file-key.sql", memoryStream);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ListObjectsAsync_WhenHasOnePage_ReturnsDataFromFirstPage()
        {
            var respose = new ListObjectsResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                IsTruncated = false,
                NextMarker = null,
                S3Objects = new List<S3Object>() 
                { 
                    new S3Object { BucketName = "test-bucket", Key = "location/file-1.csv", LastModified = DateTime.UtcNow, Size = 1 } 
                }
            };

            A.CallTo(() => _amazonS3.ListObjectsAsync(A<ListObjectsRequest>.That.Matches(m => m.BucketName == "test-bucket" && m.Prefix == "location/"), CancellationToken.None))
                .Returns(Task.FromResult(respose));

            var result = await _s3ClientService.ListObjectsAsync("s3://test-bucket/location/");

            result.Should().HaveCount(1);

            for (int i = 0; i < result.Count; i++)
            {
                var expected = respose.S3Objects[i];
                var actual = result[i];


                actual.Key.Should().Be(expected.Key);
                actual.LastModified.Should().Be(expected.LastModified);
                actual.Size.Should().Be(expected.Size);
                actual.BucketName.Should().Be(expected.BucketName);
            }
        }

        [Test]
        public async Task ListObjectsAsync_WhenHasMoreThanOnePage_ReturnsDataFromFirstPage()
        {
            var resposes = new List<ListObjectsResponse>
            {
                new ListObjectsResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    IsTruncated = true,
                    NextMarker = "Market-1",
                    S3Objects = new List<S3Object>()
                    {
                        new S3Object { BucketName = "test-bucket", Key = "location/file-1.csv", LastModified = DateTime.UtcNow, Size = 1 }
                    }
                },
                new ListObjectsResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    IsTruncated = false,
                    NextMarker = null,
                    S3Objects = new List<S3Object>()
                    {
                        new S3Object { BucketName = "test-bucket", Key = "location/file-2.csv", LastModified = DateTime.UtcNow, Size = 2 }
                    }
                }
            }.ToArray();

            A.CallTo(() => _amazonS3.ListObjectsAsync(A<ListObjectsRequest>.That.Matches(m => m.BucketName == "test-bucket" && m.Prefix == "location/"), CancellationToken.None))
                .ReturnsNextFromSequence(resposes);

            var result = await _s3ClientService.ListObjectsAsync("s3://test-bucket/location/");

            var all = resposes.SelectMany(s => s.S3Objects).ToList();

            result.Should().HaveCount(2);

            for (int i = 0; i < result.Count; i++)
            {
                var expected = all[i];
                var actual = result[i];
                
                actual.Key.Should().Be(expected.Key);
                actual.LastModified.Should().Be(expected.LastModified);
                actual.Size.Should().Be(expected.Size);
                actual.BucketName.Should().Be(expected.BucketName);
            }
        }
    }
}
