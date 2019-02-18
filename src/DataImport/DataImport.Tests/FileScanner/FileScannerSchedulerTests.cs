using System;
using System.Collections.Generic;
using System.Text;
using DataImport.File_Scanner;
using DataImport.File_Scanner.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace DataImport.Tests.FileScanner
{
    [TestFixture]
    public class FileScannerSchedulerTests
    {
        private IFileScanner _fileScanner;
        private ILogger<FileScannerScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _fileScanner = A.Fake<IFileScanner>();
            _logger = new NullLogger<FileScannerScheduler>();
        }

        [Test]
        public void Constructor_FileScanner_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScannerScheduler(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FileScannerScheduler(_fileScanner, null));
        }

        [Test]
        public void Initialise_Calls_FileScanner()
        {
            var scheduler = new FileScannerScheduler(_fileScanner, _logger);

            scheduler.Initialise();
            scheduler.Terminate();

            A.CallTo(() => _fileScanner.Scan()).MustHaveHappenedOnceExactly();
        }
    }
}
