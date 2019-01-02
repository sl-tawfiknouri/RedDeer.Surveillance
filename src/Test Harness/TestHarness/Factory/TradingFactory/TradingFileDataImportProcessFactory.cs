using System;
using DomainV2.Files;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class TradingFileDataImportProcessFactory : ITradingFileDataImportProcessFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingFileDataImportProcessFactory(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public TradingFileDataImportProcess Build(string filePath)
        {
            return new TradingFileDataImportProcess(
                _appFactory.Logger,
                new TradeFileCsvToOrderMapper(), 
                filePath);
        }
    }
}
