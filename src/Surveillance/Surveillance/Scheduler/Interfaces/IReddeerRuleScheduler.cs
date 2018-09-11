using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerRuleScheduler
    {
        Task Execute(ScheduledExecution execution);
        void Initiate();
        void Terminate();
    }
}