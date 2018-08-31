using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingSpoofingFactory
{
    public interface ITradingSpoofingProcessFactory
    {
        IOrderDataGenerator Create();
    }
}