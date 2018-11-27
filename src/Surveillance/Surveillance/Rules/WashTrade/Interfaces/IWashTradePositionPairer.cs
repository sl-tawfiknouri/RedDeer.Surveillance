using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.RuleParameters.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<PositionCluster> PairUp(List<TradeOrderFrame> trades, IWashTradeRuleParameters parameters);
    }
}