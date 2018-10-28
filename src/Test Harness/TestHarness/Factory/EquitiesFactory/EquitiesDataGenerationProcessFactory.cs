using System;
using NLog;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesDataGenerationProcessFactory : IEquitiesDataGenerationProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesDataGenerationProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquitiesDataGenerationMarkovProcess Build()
        {
            return new EquitiesDataGenerationMarkovProcess(
                new MarkovEquityStrategy(4, 5, 0.05m),
                TimeSpan.FromMinutes(15),
                _logger);
        }
    }
}
