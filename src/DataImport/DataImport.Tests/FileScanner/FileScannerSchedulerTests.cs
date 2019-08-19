namespace DataImport.Tests.FileScanner
{
    using System;

    using DataImport.File_Scanner;
    using DataImport.File_Scanner.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    [TestFixture]
    public class FileScannerSchedulerTests
    {
        private IFileScanner _fileScanner;

        private ILogger<FileScannerScheduler> _logger;

        [Test]
        public void Constructor_FileScanner_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScannerScheduler(null, this._logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScannerScheduler(this._fileScanner, null));
        }

        [Test]
        public void Initialise_Calls_FileScanner()
        {
            var scheduler = new FileScannerScheduler(this._fileScanner, this._logger);

            scheduler.Initialise();
            scheduler.Terminate();

            A.CallTo(() => this._fileScanner.Scan()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._fileScanner = A.Fake<IFileScanner>();
            this._logger = new NullLogger<FileScannerScheduler>();
        }
    }
}