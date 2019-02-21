﻿using System;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Processes
{
    /// <summary>
    /// Tracks side effects
    /// </summary>
    public class SystemProcessOperationUploadFile : ISystemProcessOperationUploadFile
    {
        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; set; }

        public string SystemProcessId { get; set; }

        public int SystemProcessOperationId { get; set; }

        public string FilePath { get; set; }

        public int FileType { get; set; }

        public DateTime FileUploadTime { get; set; }

        public override string ToString()
        {
            return $"SystemProcessOperationUploadFile | Id {Id} | SystemProcessId {SystemProcessId} | SystemProcessOperationId {SystemProcessOperationId} | FilePath {FilePath} | FileType {FileType}";
        }
    }
}
