using NLog;
using System;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesProcessFactory : IEquitiesProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquityDataGenerator Create()
        {
            var equityDataStrategy = new MarkovEquityStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = 
                new EquitiesMarkovProcess(
                    nasdaqInitialiser,
                    equityDataStrategy,
                    _logger);

            return equityDataGenerator;
        }
    }
}
