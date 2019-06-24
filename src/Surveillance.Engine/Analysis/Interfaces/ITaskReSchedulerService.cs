using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;

namespace Surveillance.Engine.Rules.Analysis.Interfaces
{
    public interface ITaskReSchedulerService
    {
        Task RescheduleFutureExecution(ScheduledExecution execution);
    }
}