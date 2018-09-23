namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderCachedMessageSender
    {
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}