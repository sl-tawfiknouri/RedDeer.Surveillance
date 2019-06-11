namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    public interface IQueueDelayedRuleSchedulerSubscriber
    {
        void Initiate();
        void Terminate();
    }
}