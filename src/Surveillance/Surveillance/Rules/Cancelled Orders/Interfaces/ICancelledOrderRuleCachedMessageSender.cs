
// ReSharper disable UnusedMember.Global

namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        int Flush();
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}