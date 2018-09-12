using System;
using Domain.Equity.Frames;
using NLog;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesFileRelayProcessFactory : IEquitiesFileRelayProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesFileRelayProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquityDataGenerator Create(string filePath)
        {
            return new EquitiesFileRelayProcess(filePath, _logger, new SecurityCsvToDtoMapper(null));
        }
    }
}
