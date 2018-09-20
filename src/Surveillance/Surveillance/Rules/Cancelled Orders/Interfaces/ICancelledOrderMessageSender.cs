using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        void Send(ITradePosition tradePosition, ICancelledOrderRuleBreach ruleBreach, ICancelledOrderRuleParameters parameters);
    }
}