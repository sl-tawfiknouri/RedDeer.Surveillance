using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcess : ISystemProcess
    {
        public SystemProcess()
        {
        }

        [Key]
        public string Id { get; set; }

        public DateTime? InstanceInitiated { get; set; }
        public DateTime? Heartbeat { get; set; }
        public string MachineId { get; set; }
        public string ProcessId { get; set; }
        public int SystemProcessTypeId { get; set; }
    }
}
