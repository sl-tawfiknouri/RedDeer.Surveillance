using Domain.Equity.Trading.Streams.Interfaces;
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

        public void RegisterTradingRules(ITradeOrderStream stream)
        {
            RegisterProhibitedAssetRule(stream);
        }

        public void RegisterProhibitedAssetRule(ITradeOrderStream stream)
        {
            if (stream == null)
            {
                return;
            }

            stream.Subscribe(_prohibitedAssetTradingRule);
        }
    }
}
