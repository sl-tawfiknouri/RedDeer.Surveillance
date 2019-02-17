namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    public interface IQueueReddeerDistributedRuleSubscriber
    {
        void Initiate();
        void Terminate();
    }
}