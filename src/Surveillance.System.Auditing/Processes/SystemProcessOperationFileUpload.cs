using System;
using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Processes
{
    /// <summary>
    /// Tracks side effects
    /// </summary>
    public class SystemProcessOperationFileUpload : ISystemProcessOperationFileUpload
    {
        private readonly ISystemProcessOperation _systemOperation;

        public SystemProcessOperationFileUpload(ISystemProcessOperation systemOperation)
        {
            _systemOperation = systemOperation ?? throw new ArgumentNullException(nameof(systemOperation));
        }

        /// <summary>
        /// Primary key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Foreign Key to Operation
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// When the file was uploaded
        /// </summary>
        public DateTime? FileUploadTime { get; set; }

        /// <summary>
        /// The name of the file provided to us
        /// </summary>
        public string FileName { get; set; }
    }
}
