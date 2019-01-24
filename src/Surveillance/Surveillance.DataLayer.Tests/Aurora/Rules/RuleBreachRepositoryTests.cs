using System;
using System.Threading.Tasks;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Rules;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Rules
{
    [TestFixture]
    public class RuleBreachRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<RuleBreachRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = new NullLogger<RuleBreachRepository>();
        }

        [Test]
        [Explicit("db integration")]
        public async Task Create_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var caseMessage =
                new RuleBreach(
                    100l,
                    "rule-1", 
                    "correlation-id",
                    true, 
                    DateTime.UtcNow, 
                    "case-title", 
                    "case-description", 
                    "xlon",
                    DateTime.UtcNow, 
                    DateTime.UtcNow, 
                    "entspb", 
                    "RD00",
                    "0");

            await repo.Create(caseMessage);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var result = await repo.Get("2");
        }
    }
}
