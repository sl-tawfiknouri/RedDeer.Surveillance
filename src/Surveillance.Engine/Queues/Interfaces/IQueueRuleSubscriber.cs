namespace Surveillance.Engine.Rules.Queues.Interfaces
{
    public interface IQueueRuleSubscriber
    {
        void Initiate();
        void Terminate();
    }
}