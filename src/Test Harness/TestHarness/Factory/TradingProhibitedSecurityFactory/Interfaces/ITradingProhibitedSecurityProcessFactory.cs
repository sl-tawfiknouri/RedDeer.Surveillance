using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingProhibitedSecurityFactory.Interfaces
{
    public interface ITradingProhibitedSecurityProcessFactory
    {
        IOrderDataGenerator Create();
    }
}