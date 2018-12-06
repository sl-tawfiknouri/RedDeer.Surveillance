using System.Collections.Generic;
using DomainV2.Trading;
using Surveillance.RuleParameters.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<PositionCluster> PairUp(List<Order> trades, IWashTradeRuleParameters parameters);
    }
}