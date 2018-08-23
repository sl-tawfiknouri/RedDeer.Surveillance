using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Factory.TradingProhibitedSecurityFactory
{
    public interface ITradingProhibitedSecurityProcessFactory
    {
        IOrderDataGenerator Create();
    }
}