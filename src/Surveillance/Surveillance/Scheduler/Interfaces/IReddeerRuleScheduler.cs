using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Scheduler.Interfaces
{
    public interface IReddeerRuleScheduler
    {
        // ReSharper disable once UnusedMember.Global
        Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
        void Initiate();
        void Terminate();
    }
}