using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engines.Interfaces
{
    public interface IAnalysisEngine
    {
        Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}