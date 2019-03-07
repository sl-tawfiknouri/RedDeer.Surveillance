using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    public interface IWashTradeRuleBreach : IRuleBreach
    {
        IWashTradeRuleParameters EquitiesParameters { get; }
        WashTradeRuleBreach.WashTradeAveragePositionBreach AveragePositionBreach { get; }
        WashTradeRuleBreach.WashTradePairingPositionBreach PairingPositionBreach { get; }
        WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }
    }
}