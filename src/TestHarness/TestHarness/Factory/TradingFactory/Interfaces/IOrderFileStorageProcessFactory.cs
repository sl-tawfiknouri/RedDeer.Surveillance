namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using TestHarness.Engine.OrderStorage.Interfaces;

    public interface IOrderFileStorageProcessFactory
    {
        IOrderFileStorageProcess Build(string directory);
    }
}