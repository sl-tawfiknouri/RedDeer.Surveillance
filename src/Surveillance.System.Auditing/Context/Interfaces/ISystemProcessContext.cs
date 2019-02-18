using System;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.Context.Interfaces
{
    public interface ISystemProcessContext
    {
        ISystemProcessOperationContext CreateOperationContext();
        ISystemProcessOperationContext CreateAndStartOperationContext();
        void StartEvent(ISystemProcess systemProcess);
        void UpdateHeartbeat();
        ISystemProcess SystemProcess();
        void EventException(Exception e);
    }
}