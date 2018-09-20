namespace Surveillance.Rules.Cancelled_Orders.Interfaces
{
    public interface ICancelledOrderPositionDeDuplicator
    {
        void Send(CancelledOrderMessageSenderParameters parameters);
    }
}