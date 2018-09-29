using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ICancelledOrderRuleFactory
    {
        ICancelledOrderRule Build(ICancelledOrderRuleParameters parameters);
    }
}