
// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        void Flush();
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}