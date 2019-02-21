using System.Linq;
using System.Threading.Tasks;
using Domain.Trading;
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
    public class RuleBreachOrdersRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<RuleBreachOrdersRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = new NullLogger<RuleBreachOrdersRepository>();
        }

        [Test]
        [Explicit("db integration")]
        public async Task Create_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachOrdersRepository(connectionStringFactory, _logger);

            var breaches = Enumerable.Range(0, 15000).Select(i => new RuleBreachOrder("20", i.ToString())).ToList();

            await repo.Create(breaches);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new RuleBreachOrdersRepository(connectionStringFactory, _logger);

            var result = await repo.Get("20");
        }
    }
}
