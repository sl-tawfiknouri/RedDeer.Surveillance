namespace Surveillance.Engine.Rules.Analysis.Interfaces
{
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IAnalysisEngine
    {
        Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}