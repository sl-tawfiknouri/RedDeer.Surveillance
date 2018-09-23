namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}