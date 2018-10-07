using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessContext
    {
        ISystemProcessOperationContext CreateOperationContext();
        ISystemProcessOperationContext CreateAndStartOperationContext();
        void StartEvent(ISystemProcess systemProcess);
        void UpdateHeartbeat();
    }
}