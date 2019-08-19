namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    using System;

    public interface ISystemProcessOperationUploadFile
    {
        string FilePath { get; set; }

        int FileType { get; set; }

        DateTime FileUploadTime { get; set; }

        int Id { get; set; }

        string SystemProcessId { get; set; }

        int SystemProcessOperationId { get; set; }
    }
}