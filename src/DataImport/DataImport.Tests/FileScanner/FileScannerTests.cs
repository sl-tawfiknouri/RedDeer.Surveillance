using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

namespace DataImport.Tests.FileScanner
{
    [TestFixture]
    public class FileScannerTests
    {
        private ISystemProcessOperationUploadFileRepository _uploadFileRepository;
        private ILogger<File_Scanner.FileScanner> _logger;

        [SetUp]
        public void Setup()
        {
            _uploadFileRepository = A.Fake<ISystemProcessOperationUploadFileRepository>();
            _logger = new NullLogger<File_Scanner.FileScanner>();
        }

        [Test]
        public void Constructor_Null_UploadFileRepository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new File_Scanner.FileScanner(null, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new File_Scanner.FileScanner(_uploadFileRepository, null));
        }

        [Test]
        public async Task Scan_Calls_UploadFileRepository()
        {
            var scanner = new File_Scanner.FileScanner(_uploadFileRepository, _logger);

            await scanner.Scan();

            A
                .CallTo(() => _uploadFileRepository.GetOnDate(A<DateTime>.Ignored))
                .MustHaveHappenedOnceExactly();
        }
    }
}
