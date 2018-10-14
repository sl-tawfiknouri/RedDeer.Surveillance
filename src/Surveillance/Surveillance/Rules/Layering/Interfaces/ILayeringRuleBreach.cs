using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringRuleBreach : IRuleBreach
    {
        ILayeringRuleParameters Parameters { get; }
        bool BidirectionalTradeBreach { get; }
        bool DailyVolumeTradeBreach { get; }
        bool WindowVolumeTradeBreach { get; }
        bool PriceMovementBreach { get; }
    }
}
