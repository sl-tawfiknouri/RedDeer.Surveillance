using System.Collections.Generic;
using Domain.Trades.Orders;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradePositionPairer
    {
        IReadOnlyCollection<WashTradePositionPairer.PositionPair> PairUp(List<TradeOrderFrame> trades, IWashTradeRuleParameters parameters);
    }
}