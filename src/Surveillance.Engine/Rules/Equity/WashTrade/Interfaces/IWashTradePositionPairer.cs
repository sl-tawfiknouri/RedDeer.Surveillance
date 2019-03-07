using System.Collections.Generic;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<PositionCluster> PairUp(List<Order> trades, IWashTradeRuleEquitiesParameters equitiesParameters);
    }
}