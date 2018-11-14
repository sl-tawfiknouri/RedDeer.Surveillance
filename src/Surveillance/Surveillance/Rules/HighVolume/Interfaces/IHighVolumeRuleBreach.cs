using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.HighVolume.Interfaces
{
    public interface IHighVolumeRuleBreach : IRuleBreach
    {
        IHighVolumeRuleParameters Parameters { get; }

        HighVolumeRuleBreach.BreachDetails DailyBreach { get; }
        HighVolumeRuleBreach.BreachDetails WindowBreach { get; }
        HighVolumeRuleBreach.BreachDetails MarketCapBreach { get; }

        int TotalOrdersTradedInWindow { get; }
    }
}
