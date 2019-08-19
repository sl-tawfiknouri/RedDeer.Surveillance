namespace Surveillance.Auditing.DataLayer.Processes
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public class SystemProcess : ISystemProcess
    {
        public bool CancelRuleQueueDeletedFlag { get; set; }

        /// <summary>
        ///     The last time the instance issued a heart beat
        /// </summary>
        public DateTime Heartbeat { get; set; }

        /// <summary>
        ///     A composite of machine - process and datetime the process began at
        ///     This is the primary key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     The time the instance began registering as a system process
        /// </summary>
        public DateTime InstanceInitiated { get; set; }

        /// <summary>
        ///     Machine name that the process is executing on
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        ///     The OS identifier for the process
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        ///     The type of system process i.e. surveillance or data import
        /// </summary>
        public SystemProcessType SystemProcessType { get; set; }

        public string GenerateInstanceId()
        {
            // ReSharper disable once UseFormatSpecifierInInterpolation
            return $"{this.ProcessId}-{this.InstanceInitiated.ToString("yyyy/MM/dd hh:mm:ss")}";
        }

        public override string ToString()
        {
            return
                $"SystemProcess | Id {this.Id} | InstanceInitiated {this.InstanceInitiated} | MachineId {this.MachineId} | ProcessId {this.ProcessId} | SystemProcessType {this.SystemProcessType} | Heartbeat {this.Heartbeat} | InstanceId {this.GenerateInstanceId()} | CancelQueueFlag {this.CancelRuleQueueDeletedFlag}";
        }
    }
}