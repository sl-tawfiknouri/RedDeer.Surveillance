using Surveillance.Rules.Cancelled_Orders.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ICancelledOrderRuleFactory
    {
        ICancelledOrderRule Build();
    }
}