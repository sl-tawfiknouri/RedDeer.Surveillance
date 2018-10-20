
// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        int Flush();
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}