using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile;
using DataImport.Integration.Tests.ObjectGraphs;
using DataImport.Managers;
using DataImport.Recorders;
using DomainV2.Files;
using DomainV2.Streams;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.System.Auditing.Context;
using Utilities.Disk_IO;

namespace DataImport.Integration.Tests.Validation
{
    [TestFixture]
    public class ValidationTestRunner
    {
        private readonly IReadOnlyCollection<ValidationFile> _validationFiles;
        private readonly IUploadTradeFileMonitor _uploadTradeFileMonitor;

        public ValidationTestRunner()
        {
            var subPath = Path.Combine("Validation", "Files");
            var filesDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, subPath);

            _validationFiles = new List<ValidationFile>
            {
                new ValidationFile(
                    Path.Combine(filesDirectory, "tradevalidation1success.csv"),
                    true,
                    6,
                    new List<Func<Order, bool>>
                    {
                        a => a.OrderId == "943191f1-4144-4b0a-8a48-3c00a2ee20e7",
                        a => a.OrderId == "9e03994a-c681-4851-b0fa-0da721c0a880",
                        a => a.OrderId == "e21fd385-c940-46fa-83c8-570b1f0207f3",
                        a => a.OrderId == "11ced167-82ac-46de-bc78-11dadf689033",
                        a => a.OrderId == "f1aadd18-d923-4a9f-9d1e-3c5e60dad40a",
                        a => a.OrderId == "6afdc02d-c1fd-4782-ab8a-9fd97ac4b698"
                    })
            };
        }

        [Test]
        public void RunAllValidationTests()
        {
            if (!_validationFiles.Any())
            {
                return;
            }

            var graph = new TradeOrderStreamManagerGraph();

            foreach (var file in _validationFiles)
            {
                // ensure file is in the right location
                Assert.IsTrue(File.Exists(file.Path));

                // now pass it through to the data import app
                var manager = graph.Build();

                var ordersList = new List<Order>();
                A.CallTo(() => graph.OrdersRepository.Create(A<Order>.Ignored)).Invokes(i => ordersList.Add((Order)i.Arguments[0]));

                var fileMonitor = manager.Initialise();

                var processFile = fileMonitor.ProcessFile(file.Path);
                
                Assert.AreEqual(processFile, file.Success);

                A.CallTo(() => graph.OrdersRepository.Create(A<Order>.Ignored)).MustHaveHappenedANumberOfTimesMatching(n => n == file.SuccessfulRows);

                // positive, any conditions
                foreach (var cond in file.RowAssertions)
                {
                    Assert.IsTrue(ordersList.Any(cond));
                }
            }
        }
    }
}
