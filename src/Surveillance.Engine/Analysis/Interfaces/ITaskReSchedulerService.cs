namespace Surveillance.Engine.Rules.Analysis.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    public interface ITaskReSchedulerService
    {
        Task RescheduleFutureExecution(ScheduledExecution execution);
    }
}