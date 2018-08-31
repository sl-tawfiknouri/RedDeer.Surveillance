using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.Rules.ProhibitedAssetTradingRule;
using System;

namespace Surveillance.Rules
{
    public class RuleManager : IRuleManager
    {
        private IProhibitedAssetTradingRule _prohibitedAssetTradingRule;

        public RuleManager(IProhibitedAssetTradingRule prohibitedAssetTradingRule)
        {
            _prohibitedAssetTradingRule = prohibitedAssetTradingRule 
                ?? throw new ArgumentNullException(nameof(prohibitedAssetTradingRule));
        }

        public void RegisterTradingRules(ITradeOrderStream<TradeOrderFrame> stream)
        {
            RegisterProhibitedAssetRule(stream);
        }

        public void RegisterEquityRules(IStockExchangeStream stream)
        {

        }

        public void RegisterProhibitedAssetRule(ITradeOrderStream<TradeOrderFrame> stream)
        {
            if (stream == null)
            {
                return;
            }

            stream.Subscribe(_prohibitedAssetTradingRule);
        }
    }
}
