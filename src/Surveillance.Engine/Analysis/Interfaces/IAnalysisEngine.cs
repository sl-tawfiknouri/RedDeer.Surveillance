using System.Threading.Tasks;
using Domain.Scheduling;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Analysis.Interfaces
{
    public interface IAnalysisEngine
    {
        Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}