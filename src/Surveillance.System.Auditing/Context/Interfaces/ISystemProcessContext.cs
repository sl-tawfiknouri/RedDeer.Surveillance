namespace Surveillance.Auditing.Context.Interfaces
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessContext
    {
        ISystemProcessOperationContext CreateAndStartOperationContext();

        ISystemProcessOperationContext CreateOperationContext();

        void EventException(Exception e);

        void StartEvent(ISystemProcess systemProcess);

        ISystemProcess SystemProcess();

        void UpdateHeartbeat();
    }
}