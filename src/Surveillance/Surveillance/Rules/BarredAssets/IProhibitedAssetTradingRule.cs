using System;
using Domain.Equity.Trading.Orders;

namespace Surveillance.Rules.ProhibitedAssetTradingRule
{
    public interface IProhibitedAssetTradingRule : IObserver<TradeOrderFrame>
    {
    }
}