namespace DataSynchroniser.Queues.Interfaces
{
    public interface IDataRequestSubscriber
    {
        void Initiate();
        void Terminate();
    }
}