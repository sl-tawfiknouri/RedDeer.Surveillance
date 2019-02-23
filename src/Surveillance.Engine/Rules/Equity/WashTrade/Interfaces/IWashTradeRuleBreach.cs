﻿using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeRuleBreach : IRuleBreach
    {
        IWashTradeRuleEquitiesParameters EquitiesParameters { get; }
        WashTradeRuleBreach.WashTradeAveragePositionBreach AveragePositionBreach { get; }
        WashTradeRuleBreach.WashTradePairingPositionBreach PairingPositionBreach { get; }
        WashTradeRuleBreach.WashTradeClusteringPositionBreach ClusteringPositionBreach { get; }
    }
}