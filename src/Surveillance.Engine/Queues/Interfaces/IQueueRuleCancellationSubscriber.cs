namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueRuleCancellationSubscriber
    {
        void Initiate();

        void Terminate();
    }
}