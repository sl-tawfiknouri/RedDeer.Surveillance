using DataSynchroniser.Api.Bmll.Bmll;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.BmllMarketData.Interfaces;

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class BmllDataRequestsSenderManagerTests
    {
        private IBmllTimeBarApiRepository _timeBarRepository;
        private ILogger<BmllDataRequestsSenderManager> _logger;

        [SetUp]
        public void Setup()
        {
            _timeBarRepository = A.Fake<IBmllTimeBarApiRepository>();
            _logger = A.Fake<ILogger<BmllDataRequestsSenderManager>>();
        }
    }
}
