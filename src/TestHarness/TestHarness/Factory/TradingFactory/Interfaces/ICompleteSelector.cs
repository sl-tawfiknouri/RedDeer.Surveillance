using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ICompleteSelector
    {
        IOrderDataGenerator Finish();
    }
}
