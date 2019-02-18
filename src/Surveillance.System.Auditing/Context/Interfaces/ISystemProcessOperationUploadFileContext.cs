﻿using System;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationUploadFileContext
    {
        ISystemProcessOperationUploadFile FileUpload { get; }
        ISystemProcessOperationContext EndEvent();
        void EventException(Exception e);
        void EventException(string message);
        void StartEvent(ISystemProcessOperationUploadFile upload);
    }
}