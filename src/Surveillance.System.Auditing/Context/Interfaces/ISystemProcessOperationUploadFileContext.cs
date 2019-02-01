using System;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationUploadFileContext
    {
        ISystemProcessOperationContext EndEvent();
        void EventException(Exception e);
        void EventException(string message);
        void StartEvent(ISystemProcessOperationUploadFile upload);
    }
}