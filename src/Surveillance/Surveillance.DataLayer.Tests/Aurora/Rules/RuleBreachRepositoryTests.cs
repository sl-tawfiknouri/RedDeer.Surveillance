using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Surveillance.Rules;
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
                    100,
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
                    "0",
                    0,
                    "1",
                    new int[0]);

           var result = await repo.Create(caseMessage);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var result = await repo.Get("2");
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Duplicates_When_Duplicates_Reports_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var caseMessage1 =
                new RuleBreach(
                    100,
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var caseMessage2 =
                new RuleBreach(
                    101,
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var result1 = await repo.Create(caseMessage1);
            var result2 = await repo.Create(caseMessage2);

            var orderRepo = new RuleBreachOrdersRepository(connectionStringFactory, new NullLogger<RuleBreachOrdersRepository>());

            var breaches1 = Enumerable.Range(0, 50).Select(i => new RuleBreachOrder(result1.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches1);

            var breaches2 = Enumerable.Range(0, 100).Select(i => new RuleBreachOrder(result2.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches2);

            var result = await repo.HasDuplicate(result1.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Duplicates_When_Many_Duplicates_Reports_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var caseMessage1 =
                new RuleBreach(
                    300,
                    "rule-3",
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var caseMessage2 =
                new RuleBreach(
                    300,
                    "rule-3",
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var caseMessage3 =
                new RuleBreach(
                    300,
                    "rule-3",
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var result1 = await repo.Create(caseMessage1);
            var result2 = await repo.Create(caseMessage2);
            var result3 = await repo.Create(caseMessage3);

            var orderRepo = new RuleBreachOrdersRepository(connectionStringFactory, new NullLogger<RuleBreachOrdersRepository>());

            var breaches1 = Enumerable.Range(0, 50).Select(i => new RuleBreachOrder(result1.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches1);

            var breaches2 = Enumerable.Range(0, 100).Select(i => new RuleBreachOrder(result2.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches2);

            var breaches3 = Enumerable.Range(0, 100).Select(i => new RuleBreachOrder(result3.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches3);

            var result = await repo.HasDuplicate(result1.ToString());

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Duplicates_When_NoDuplicates_Reports_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachRepository(connectionStringFactory, _logger);

            var caseMessage1 =
                new RuleBreach(
                    900,
                    "rule-9",
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
                    "0",
                    0,
                    "1",
                    new int[0]);

            var result1 = await repo.Create(caseMessage1);

            var orderRepo = new RuleBreachOrdersRepository(connectionStringFactory, new NullLogger<RuleBreachOrdersRepository>());

            var breaches1 = Enumerable.Range(0, 50).Select(i => new RuleBreachOrder(result1.ToString(), i.ToString())).ToList();
            await orderRepo.Create(breaches1);

            var result = await repo.HasDuplicate(result1.ToString());

            Assert.IsFalse(result);
        }
    }
}
