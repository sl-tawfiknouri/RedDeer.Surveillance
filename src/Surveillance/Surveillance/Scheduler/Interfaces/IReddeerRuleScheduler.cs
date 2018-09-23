using System.Threading.Tasks;
using Domain.Scheduling;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerRuleScheduler
    {
        // ReSharper disable once UnusedMember.Global
        Task Execute(ScheduledExecution execution);
        void Initiate();
        void Terminate();
    }
}