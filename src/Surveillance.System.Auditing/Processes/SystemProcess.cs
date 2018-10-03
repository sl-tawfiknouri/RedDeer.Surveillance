using System;
using Surveillance.System.Auditing.Processes.Interfaces;

namespace Surveillance.System.Auditing.Processes
{
    public class SystemProcess : ISystemProcess
    {
        /// <summary>
        /// A composite of machine - process and datetime the process began at
        /// This is the primary key
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// The time the instance began registering as a system process
        /// </summary>
        public DateTime InstanceInitiated { get; set; }

        /// <summary>
        /// Machine name that the process is executing on
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// The OS identifier for the process
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// The last time the instance issued a heart beat
        /// </summary>
        public DateTime Heartbeat { get; set; }
    }
}
