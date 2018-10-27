using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
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
    }
}
