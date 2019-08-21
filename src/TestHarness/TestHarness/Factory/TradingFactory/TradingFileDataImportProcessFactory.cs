namespace TestHarness.Factory.TradingFactory
{
    using System;

    using SharedKernel.Files.Orders;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Factory.Interfaces;
    using TestHarness.Factory.TradingFactory.Interfaces;

    public class TradingFileDataImportProcessFactory : ITradingFileDataImportProcessFactory
    {
        private readonly IAppFactory _appFactory;

        public TradingFileDataImportProcessFactory(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public TradingFileDataImportProcess Build(string filePath)
        {
            return new TradingFileDataImportProcess(
                this._appFactory.Logger,
                new OrderFileToOrderSerialiser(),
                filePath);
        }
    }
}