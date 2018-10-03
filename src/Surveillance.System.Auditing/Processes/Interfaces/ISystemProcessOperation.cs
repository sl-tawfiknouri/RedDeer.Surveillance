using System;

namespace Surveillance.System.Auditing.Processes.Interfaces
{
    public interface ISystemProcessOperation
    {
        string Id { get; set; }
        string InstanceId { get; set; }
        DateTime? OperationEnd { get; set; }
        DateTime OperationStart { get; set; }
        OperationState OperationState { get; set; }

        ISystemProcessOperationFileUpload SpawnFileUpload();
    }
}