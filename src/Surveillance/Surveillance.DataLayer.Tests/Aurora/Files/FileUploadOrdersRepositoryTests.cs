namespace Surveillance.DataLayer.Tests.Aurora.Files
{
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

    [TestFixture]
    public class FileUploadOrdersRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private IConnectionStringFactory _connectionFactory;

        private ILogger<FileUploadOrdersRepository> _logger;

        [Test]
        [Explicit("Performs side effect to the database")]
        public async Task Insert_Inserts_A_Row_As_Expected()
        {
            var repository = new FileUploadOrdersRepository(this._connectionFactory, this._logger);

            var insertableIds = new List<string>
                                    {
                                        "1",
                                        "2",
                                        "3",
                                        "4",
                                        "5"
                                    };

            await repository.Create(insertableIds, 1);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._connectionFactory = new ConnectionStringFactory(this._configuration);
            this._logger = new NullLogger<FileUploadOrdersRepository>();
        }
    }
}