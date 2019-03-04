﻿using System.Collections.Generic;
using Domain.Trading;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<PositionCluster> PairUp(List<Order> trades, IWashTradeRuleEquitiesParameters equitiesParameters);
    }
}