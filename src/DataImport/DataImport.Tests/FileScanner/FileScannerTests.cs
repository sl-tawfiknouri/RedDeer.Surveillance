namespace DataImport.Tests.FileScanner
{
    using System;
    using System.Threading.Tasks;

    using DataImport.File_Scanner;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    [TestFixture]
    public class FileScannerTests
    {
        private ILogger<FileScanner> _logger;

        private ISystemProcessOperationUploadFileRepository _uploadFileRepository;

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScanner(this._uploadFileRepository, null));
        }

        [Test]
        public void Constructor_Null_UploadFileRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScanner(null, this._logger));
        }

        [Test]
        public async Task Scan_Calls_UploadFileRepository()
        {
            var scanner = new FileScanner(this._uploadFileRepository, this._logger);

            await scanner.Scan();

            A.CallTo(() => this._uploadFileRepository.GetOnDate(A<DateTime>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._uploadFileRepository = A.Fake<ISystemProcessOperationUploadFileRepository>();
            this._logger = new NullLogger<FileScanner>();
        }
    }
}