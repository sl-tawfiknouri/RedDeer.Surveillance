namespace Surveillance.DataLayer.Tests.Aurora.Tuning
{
    using System;
    using System.Threading.Tasks;

    using Domain.Surveillance.Rules.Tuning;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Tuning;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class TuningRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<TuningRepository> _logger;

        [Test]
        [Explicit("Performs side effect to the d-b")]
        public async Task Create()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new TuningRepository(factory, this._logger);

            var parameterHelper = new TestParameterHelper
                                      {
                                          Id = "123Id",
                                          Number1 = 1,
                                          Number2 = 920,
                                          Date1 = DateTime.Now,
                                          Date2 = DateTime.Now.AddDays(-5),
                                          TunedParameter = new TunedParameter<string>(
                                              "base-1",
                                              "base-2",
                                              "a-param",
                                              "b100",
                                              "b100-a-param",
                                              TuningDirection.Positive,
                                              TuningStrength.Medium)
                                      };

            var tuningPair = new TuningRepository.TuningPair(
                parameterHelper.TunedParameter,
                JsonConvert.SerializeObject(parameterHelper));

            await repo.SaveTasks(new[] { tuningPair });

            Assert.IsTrue(true);
        }

        [SetUp]
        public void TuningRepositoryTests_Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = A.Fake<ILogger<TuningRepository>>();
        }

        private class TestParameterHelper
        {
            public DateTime Date1 { get; set; }

            public DateTime Date2 { get; set; }

            public string Id { get; set; }

            public int Number1 { get; set; }

            public int Number2 { get; set; }

            public TunedParameter<string> TunedParameter { get; set; }
        }
    }
}