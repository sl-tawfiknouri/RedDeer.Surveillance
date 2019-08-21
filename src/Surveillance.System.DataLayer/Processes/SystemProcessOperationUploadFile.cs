namespace Surveillance.Auditing.DataLayer.Processes
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    /// <summary>
    ///     Tracks side effects
    /// </summary>
    public class SystemProcessOperationUploadFile : ISystemProcessOperationUploadFile
    {
        public string FilePath { get; set; }

        public int FileType { get; set; }

        public DateTime FileUploadTime { get; set; }

        /// <summary>
        ///     Database primary key
        /// </summary>
        public int Id { get; set; }

        public string SystemProcessId { get; set; }

        public int SystemProcessOperationId { get; set; }

        public override string ToString()
        {
            return
                $"SystemProcessOperationUploadFile | Id {this.Id} | SystemProcessId {this.SystemProcessId} | SystemProcessOperationId {this.SystemProcessOperationId} | FilePath {this.FilePath} | FileType {this.FileType}";
        }
    }
}