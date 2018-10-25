using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.DataLayer.Repositories.Exceptions;

namespace Surveillance.System.DataLayer.Tests.Repositories.Exceptions
{
    [TestFixture]
    public class ExceptionRepositoryTests
    {
        private ILogger<ExceptionRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ExceptionRepository>>();
        }

        [Test]
        [Explicit]
        public void ExceptionSavesToDatabase()
        {
            var config = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };
            var repository = new ExceptionRepository(new ConnectionStringFactory(config), _logger);
            var dtos = new ExceptionDto
            {
                ExceptionMessage = "hello world",
                InnerExceptionMessage = "Goodbye world",
                StackTrace = "a/b/c",
                SystemProcessId = "0",
                SystemProcessOperationId = 1
            };

            repository.Save(dtos);
        }
    }
}
