using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.Plans;
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

        public IEquitiesDataGenerationMarkovProcess Build(IReadOnlyCollection<DataGenerationPlan> plans = null)
        {
            plans = plans ?? new List<DataGenerationPlan>();
            return new EquitiesDataGenerationMarkovProcess(
                new MarkovEquityStrategy(0.1, 3, 0.001m),
                plans,
                TimeSpan.FromMinutes(15),
                _logger);
        }
    }
}
