using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationUploadFileContext
    {
        ISystemProcessOperationContext EndEvent();
        void EventException(Exception e);
        void EventException(string message);
        void StartEvent(ISystemProcessOperationUploadFile upload);
    }
}