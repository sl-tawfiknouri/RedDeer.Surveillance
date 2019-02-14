namespace Surveillance.Engine.DataCoordinator.Queues.Interfaces
{
    public interface IQueueSubscriber
    {
        void Initiate();
        void Terminate();
    }
}