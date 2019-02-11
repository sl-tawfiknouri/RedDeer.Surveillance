using System.Threading.Tasks;
using DomainV2.Scheduling;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Interfaces
{
    public interface IAnalysisEngine
    {
        Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx);
    }
}