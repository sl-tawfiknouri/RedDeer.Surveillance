using System;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Security;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesFileDataImportProcessFactory : IEquitiesFileDataImportProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesFileDataImportProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquityDataGenerator Create(string filePath)
        {
            return new EquitiesFileDataImportProcess(filePath, _logger, new SecurityCsvToDtoMapper(null));
        }
    }
}
