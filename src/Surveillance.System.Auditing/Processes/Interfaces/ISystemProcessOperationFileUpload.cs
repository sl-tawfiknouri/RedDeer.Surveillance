using System;

namespace Surveillance.System.Auditing.Processes.Interfaces
{
    public interface ISystemProcessOperationFileUpload
    {
        string FileName { get; set; }
        DateTime? FileUploadTime { get; set; }
        string Id { get; set; }
        string OperationId { get; set; }
    }
}