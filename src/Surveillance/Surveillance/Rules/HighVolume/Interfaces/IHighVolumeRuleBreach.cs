using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Interfaces;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeRuleBreach : IRuleBreach
    {
        IHighVolumeRuleParameters Parameters { get; }

        HighVolumeRuleBreach.BreachDetails DailyBreach { get; }
        HighVolumeRuleBreach.BreachDetails WindowBreach { get; }
        HighVolumeRuleBreach.BreachDetails MarketCapBreach { get; }

        long TotalOrdersTradedInWindow { get; }
    }
}
