namespace Surveillance.Engine.RuleDistributor.Queues.Interfaces
{
    public interface IQueueDistributedRuleSubscriber
    {
        void Initiate();

        void Terminate();
    }
}