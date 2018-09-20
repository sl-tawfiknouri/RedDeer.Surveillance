using Surveillance.Rules.Interfaces;
using Surveillance.Rules.ProhibitedAssets.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using System;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;

namespace Surveillance.Rules
{
    public class RuleManager : IRuleManager
    {
        private readonly IProhibitedAssetTradingRule _prohibitedAssetTradingRule;
        private readonly ISpoofingRule _spoofingRule;
        private readonly ICancelledOrderRule _cancelRule;

        public RuleManager(
            IProhibitedAssetTradingRule prohibitedAssetTradingRule,
            ISpoofingRule spoofingRule,
            ICancelledOrderRule cancelRule)
        {
            _prohibitedAssetTradingRule = prohibitedAssetTradingRule 
                ?? throw new ArgumentNullException(nameof(prohibitedAssetTradingRule));

            _spoofingRule = spoofingRule
                ?? throw new ArgumentNullException(nameof(spoofingRule));

            _cancelRule = cancelRule
                ?? throw new ArgumentNullException(nameof(cancelRule));
        }

        public void RegisterTradingRules(ITradeOrderStream<TradeOrderFrame> stream)
        {
            RegisterProhibitedAssetRule(stream);
            RegisterSpoofingRule(stream);
            RegisterCancelledOrderRule(stream);
        }

        public void RegisterEquityRules(IStockExchangeStream stream)
        {
        }

        private void RegisterProhibitedAssetRule(ITradeOrderStream<TradeOrderFrame> stream)
        {
            stream?.Subscribe(_prohibitedAssetTradingRule);
        }

        private void RegisterSpoofingRule(ITradeOrderStream<TradeOrderFrame> stream)
        {
            stream?.Subscribe(_spoofingRule);
        }

        private void RegisterCancelledOrderRule(ITradeOrderStream<TradeOrderFrame> stream)
        {
            stream?.Subscribe(_cancelRule);
        }
    }
}