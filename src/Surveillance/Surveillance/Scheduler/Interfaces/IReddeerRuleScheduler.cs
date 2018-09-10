using System.Threading.Tasks;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerRuleScheduler
    {
        Task Execute(ScheduledExecution execution);
        void Initiate();
        void Terminate();
    }
}