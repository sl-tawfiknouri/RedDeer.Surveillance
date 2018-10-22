namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderCachedMessageSender
    {
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}