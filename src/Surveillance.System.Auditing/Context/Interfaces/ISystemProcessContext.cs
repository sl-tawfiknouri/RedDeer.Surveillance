using System;
using Surveillance.Systems.DataLayer.Processes;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.Auditing.Context.Interfaces
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