using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
{
    public class SystemProcessOperation : ISystemProcessOperation
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Foreign Key
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// Operation began at
        /// </summary>
        public DateTime OperationStart { get; set; }

        /// <summary>
        /// Operation ended at
        /// </summary>
        public DateTime? OperationEnd { get; set; }

        /// <summary>
        /// Operation state
        /// </summary>
        public OperationState OperationState { get; set; }
    }
}
