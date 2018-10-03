using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessContext
    {
        ISystemProcessOperationContext CreateOperationContext();
        void StartEvent(ISystemProcess systemProcess);
    }
}