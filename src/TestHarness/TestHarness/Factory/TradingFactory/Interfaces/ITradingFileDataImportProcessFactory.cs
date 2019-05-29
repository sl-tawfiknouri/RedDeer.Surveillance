using TestHarness.Engine.OrderGenerator;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFileDataImportProcessFactory
    {
        TradingFileDataImportProcess Build(string filePath);
    }
}