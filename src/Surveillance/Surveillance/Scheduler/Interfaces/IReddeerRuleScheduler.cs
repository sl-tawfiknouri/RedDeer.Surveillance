namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerRuleScheduler
    {
        void Execute(ScheduledExecution execution);
        void Initiate();
        void Terminate();
    }
}