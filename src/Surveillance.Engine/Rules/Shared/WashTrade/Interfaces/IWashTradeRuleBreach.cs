namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IWashTradeRuleBreach : IRuleBreach
    {
        WashTradeRuleBreach.WashTradeAveragePositionBreach AveragePositionBreach { get; }

        WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }

        IWashTradeRuleParameters EquitiesParameters { get; }
    }
}