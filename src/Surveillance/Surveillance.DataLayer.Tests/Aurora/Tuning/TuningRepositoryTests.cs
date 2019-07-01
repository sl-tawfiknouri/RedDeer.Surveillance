using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Tuning;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Tests.Aurora.Tuning
{
    public class TuningRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<TuningRepository> _logger;
        private ISystemProcessOperationContext _opCtx;



        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new TuningRepository(factory, _logger);



            Assert.IsTrue(true);
        }
    }
}
