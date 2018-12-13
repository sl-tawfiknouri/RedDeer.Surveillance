using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
{
    public class SystemProcessOperation : ISystemProcessOperation
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign Key
        /// </summary>
        public string SystemProcessId { get; set; }

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

        public override string ToString()
        {
            return $"SystemProcessOperation | Id {Id} | SystemProcessId {SystemProcessId} | OperationStart {OperationStart} | OperationEnd {OperationEnd} | OperationState {OperationState}";
        }
    }
}
