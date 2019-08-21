namespace TestHarness.Factory.EquitiesFactory
{
    using System;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Security;

    using TestHarness.Engine.EquitiesGenerator;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Factory.EquitiesFactory.Interfaces;

    public class EquitiesFileDataImportProcessFactory : IEquitiesFileDataImportProcessFactory
    {
        private readonly ILogger _logger;

        public EquitiesFileDataImportProcessFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEquityDataGenerator Create(string filePath)
        {
            return new EquitiesFileDataImportProcess(filePath, this._logger, new SecurityCsvToDtoMapper(null));
        }
    }
}