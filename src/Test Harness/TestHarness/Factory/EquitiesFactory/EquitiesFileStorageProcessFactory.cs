using System;
using DomainV2.Equity.TimeBars;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesStorage;
using TestHarness.Engine.EquitiesStorage.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesFileStorageProcessFactory 
        : IEquitiesFileStorageProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesFileStorageProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquityDataStorage Create(string path)
        {
            return new EquitiesFileStorageProcess(path, _logger, new DtoToSecurityCsvMapper(null));
        }
    }
}
