using System.Collections.Generic;
using DomainV2.Trading;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.Rules.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<PositionCluster> PairUp(List<Order> trades, IWashTradeRuleParameters parameters);
    }
}