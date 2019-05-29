using TestHarness.Engine.OrderStorage.Interfaces;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface IOrderFileStorageProcessFactory
    {
        IOrderFileStorageProcess Build(string directory);
    }
}