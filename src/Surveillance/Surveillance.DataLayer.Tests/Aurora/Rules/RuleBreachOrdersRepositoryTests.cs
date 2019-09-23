namespace Surveillance.DataLayer.Tests.Aurora.Rules
{
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

    [TestFixture]
    public class RuleBreachOrdersRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<RuleBreachOrdersRepository> _logger;

        [Test]
        [Explicit("db integration")]
        public async Task Create_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new RuleBreachOrdersRepository(connectionStringFactory, this._logger);

            var breaches = Enumerable.Range(0, 15000).Select(i => new RuleBreachOrder("20", i.ToString())).ToList();

            await repo.Create(breaches);
        }

        [Test]
        [Explicit("db integration")]
        public async Task Get_Creates_AsExpected()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new RuleBreachOrdersRepository(connectionStringFactory, this._logger);

            var result = await repo.Get("20");
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = new NullLogger<RuleBreachOrdersRepository>();
        }
    }
}