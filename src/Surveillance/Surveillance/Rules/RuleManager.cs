using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.Rules.ProhibitedAssetTradingRule;
using Surveillance.Rules.Spoofing.Interfaces;
using System;

namespace Surveillance.Rules
{
    public class RuleManager : IRuleManager
    {
        private IProhibitedAssetTradingRule _prohibitedAssetTradingRule;
        private ISpoofingRule _spoofingRule;

        public RuleManager(
            IProhibitedAssetTradingRule prohibitedAssetTradingRule,
            ISpoofingRule spoofingRule)
        {
            _prohibitedAssetTradingRule = prohibitedAssetTradingRule 
                ?? throw new ArgumentNullException(nameof(prohibitedAssetTradingRule));

            _spoofingRule = spoofingRule
                ?? throw new ArgumentNullException(nameof(spoofingRule));
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
            stream.Subscribe(_spoofingRule);
        }
    }
}