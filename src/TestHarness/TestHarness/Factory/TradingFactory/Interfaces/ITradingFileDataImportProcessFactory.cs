namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using TestHarness.Engine.OrderGenerator;

    public interface ITradingFileDataImportProcessFactory
    {
        TradingFileDataImportProcess Build(string filePath);
    }
}