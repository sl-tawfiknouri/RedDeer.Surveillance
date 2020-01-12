using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Tests.Services
{
    public class FilePreProcessorServiceUnitTests
    {
        private IS3ClientService _s3ClientService = null;
        private ICSVService _csvService = null;


        private FilePreProcessorService _filePreProcessorService;

        [SetUp]
        public void SetUp()
        {
            _s3ClientService = A.Fake<IS3ClientService>();
            _csvService = A.Fake<ICSVService>();

            _filePreProcessorService = new FilePreProcessorService(_s3ClientService, _csvService, new NullLogger<FilePreProcessorService>());
        }

        [Test]
        public async Task PreProcessAsync_WhenModifiedCsv_PutObjectStreamMustNotBeCalled()
        {
            var data = new FilePreProcessorData()
            {
                Minutes = 10,
                S3Locations = new string[] { "S3://test-bucket/location/" }
            };

            var s3ObjectModels = new List<S3ObjectModel>
            {
                new S3ObjectModel {  BucketName = "test-bucket", Key = "location/file-1.csv", LastModified = DateTime.UtcNow }
            };

            A.CallTo(() => _s3ClientService.ListObjectsAsync(A<string>.That.Matches(m => m.Equals(data.S3Locations[0]))))
               .Returns(Task.FromResult(s3ObjectModels));

            var s3ObjectStream = new MemoryStream();

            A.CallTo(() => _s3ClientService.GetObjectStream(A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].BucketName)), A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].Key)), A<string>.That.Matches(m => m == null)))
               .Returns(Task.FromResult(s3ObjectStream));

            var newLinesReplaced = 0;

            A.CallTo(() => _csvService.ReplaceNewLines(A<string>.That.Matches(m => m.Equals($"{ s3ObjectModels[0].BucketName}/{s3ObjectModels[0].Key}")), A<MemoryStream>.That.Matches(m => m.Equals(s3ObjectStream)), A<MemoryStream>.Ignored))
               .Returns(newLinesReplaced);

            var result = await _filePreProcessorService.PreProcessAsync(data);

            A.CallTo(() => _s3ClientService.PutObjectStream(A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].BucketName)), A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].Key)), A<Stream>.Ignored))
               .MustNotHaveHappened();

            result.Should().BeTrue();
        }


        [Test]
        public async Task PreProcessAsync_WhenModifiedCsv_PutObjectStreamMustBeCalled()
        {
            var data = new FilePreProcessorData()
            {
                Minutes = 10,
                S3Locations = new string[] { "S3://test-bucket/location/" }
            };

            var s3ObjectModels = new List<S3ObjectModel>
            {
                new S3ObjectModel {  BucketName = "test-bucket", Key = "location/file-1.csv", LastModified = DateTime.UtcNow }
            };

            A.CallTo(() => _s3ClientService.ListObjectsAsync(A<string>.That.Matches(m => m.Equals(data.S3Locations[0]))))
               .Returns(Task.FromResult(s3ObjectModels));

            var s3ObjectStream = new MemoryStream();

            A.CallTo(() => _s3ClientService.GetObjectStream(A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].BucketName)), A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].Key)), A<string>.That.Matches( m => m == null)))
               .Returns(Task.FromResult(s3ObjectStream));

            var newLinesReplaced = 4;

            A.CallTo(() => _csvService.ReplaceNewLines(A<string>.That.Matches(m => m.Equals($"{ s3ObjectModels[0].BucketName}/{s3ObjectModels[0].Key}")), A<MemoryStream>.That.Matches(m => m.Equals(s3ObjectStream)), A<MemoryStream>.Ignored))
               .Returns(newLinesReplaced);

            var result = await _filePreProcessorService.PreProcessAsync(data);

            A.CallTo(() => _s3ClientService.PutObjectStream(A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].BucketName)), A<string>.That.Matches(m => m.Equals(s3ObjectModels[0].Key)), A<Stream>.Ignored))
               .MustHaveHappenedOnceExactly();

            result.Should().BeTrue();
        }
    }
}
