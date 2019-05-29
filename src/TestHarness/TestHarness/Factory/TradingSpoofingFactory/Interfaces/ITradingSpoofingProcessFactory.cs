using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingSpoofingFactory.Interfaces
{
    public interface ITradingSpoofingProcessFactory
    {
        IOrderDataGenerator Create();
    }
}