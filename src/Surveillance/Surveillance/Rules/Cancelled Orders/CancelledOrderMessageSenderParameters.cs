using Domain.Equity.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderMessageSenderParameters
    {
        public CancelledOrderMessageSenderParameters(ISecurityIdentifiers identifiers)
        {
            Identifiers = identifiers;
        }

        public ISecurityIdentifiers Identifiers { get; }
        public ITradePosition TradePosition { get; set; }
        public ICancelledOrderRuleBreach RuleBreach { get; set; }
        public ICancelledOrderRuleParameters Parameters { get; set; }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Identifiers?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CancelledOrderMessageSenderParameters castObj))
            {
                return false;
            }

            return Equals(this.Identifiers, castObj.Identifiers);
        }
    }
}