// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        int Flush();

        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}