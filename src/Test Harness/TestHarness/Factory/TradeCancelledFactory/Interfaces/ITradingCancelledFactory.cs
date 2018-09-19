using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradeCancelledFactory.Interfaces
{
    public interface ITradingCancelledFactory
    {
        IOrderDataGenerator Create();
    }
}