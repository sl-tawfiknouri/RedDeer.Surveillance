namespace Surveillance.Auditing.DataLayer.Processes
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public class SystemProcessOperation : ISystemProcessOperation
    {
        /// <summary>
        ///     Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Operation ended at
        /// </summary>
        public DateTime? OperationEnd { get; set; }

        /// <summary>
        ///     Operation began at
        /// </summary>
        public DateTime OperationStart { get; set; }

        /// <summary>
        ///     Operation state
        /// </summary>
        public OperationState OperationState { get; set; }

        /// <summary>
        ///     Foreign Key
        /// </summary>
        public string SystemProcessId { get; set; }

        public override string ToString()
        {
            return
                $"SystemProcessOperation | Id {this.Id} | SystemProcessId {this.SystemProcessId} | OperationStart {this.OperationStart} | OperationEnd {this.OperationEnd} | OperationState {this.OperationState}";
        }
    }
}