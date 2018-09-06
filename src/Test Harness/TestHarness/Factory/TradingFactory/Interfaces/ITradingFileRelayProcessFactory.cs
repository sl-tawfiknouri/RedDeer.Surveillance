using TestHarness.Engine.OrderGenerator;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFileRelayProcessFactory
    {
        TradingFileRelayProcess Build(string filePath);
    }
}