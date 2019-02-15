using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Files;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Files
{
    [TestFixture]
    public class FileUploadOrderAllocationsRepositoryTests
    {
        private IConnectionStringFactory _connectionFactory;
        private IDataLayerConfiguration _configuration;
        private ILogger<FileUploadOrderAllocationRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _connectionFactory = new ConnectionStringFactory(_configuration);
            _logger = new NullLogger<FileUploadOrderAllocationRepository>();
        }

        [Test]
        [Explicit("Performs side effect to the database")]
        public async Task Insert_Inserts_A_Row_As_Expected()
        {
            var repository = new FileUploadOrderAllocationRepository(_connectionFactory, _logger);

            var insertableIds = new List<int> { 1, 2, 3, 4, 5};

            await repository.Create(insertableIds, 1);
        }
    }
}
