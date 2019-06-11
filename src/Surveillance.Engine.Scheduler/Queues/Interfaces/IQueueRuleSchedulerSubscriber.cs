namespace Surveillance.Engine.Scheduler.Queues.Interfaces
{
    public interface IQueueRuleSchedulerSubscriber
    {
        void Initiate();
        void Terminate();
    }
}