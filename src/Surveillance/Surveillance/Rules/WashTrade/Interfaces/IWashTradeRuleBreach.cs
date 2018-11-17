using Surveillance.Rules.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using static Surveillance.Rules.WashTrade.WashTradeRuleBreach;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeRuleBreach : IRuleBreach
    {
        IWashTradeRuleParameters Parameters { get; }
        WashTradeAveragePositionBreach AveragePositionBreach { get; }
        WashTradePairingPositionBreach PairingPositionBreach { get; }
    }
}