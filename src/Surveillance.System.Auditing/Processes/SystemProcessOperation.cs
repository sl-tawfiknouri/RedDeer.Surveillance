using System;
using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Processes
{
    public class SystemProcessOperation : ISystemProcessOperation
    {
        private readonly ISystemProcess _parent;

        public SystemProcessOperation(ISystemProcess parent)
        {
            _parent = parent ?? throw new ArgumentNullException();
        }

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
