
// ReSharper disable UnusedMember.Global

namespace Surveillance.Engine.Rules.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        int Flush();
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}