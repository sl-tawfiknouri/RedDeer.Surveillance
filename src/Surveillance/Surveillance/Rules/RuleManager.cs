using Surveillance.Rules.Interfaces;
using Surveillance.Rules.ProhibitedAssets.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;

namespace Surveillance.Rules
{
    public class RuleManager : IRuleManager
    {
        private readonly IProhibitedAssetTradingRule _prohibitedAssetTradingRule;
        private readonly ISpoofingRule _spoofingRule;

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