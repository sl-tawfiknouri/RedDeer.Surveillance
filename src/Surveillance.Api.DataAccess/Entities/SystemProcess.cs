namespace Surveillance.Api.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcess : ISystemProcess
    {
        public DateTime? Heartbeat { get; set; }

        [Key]
        public string Id { get; set; }

        public DateTime? InstanceInitiated { get; set; }

        public string MachineId { get; set; }

        public string ProcessId { get; set; }

        public int SystemProcessTypeId { get; set; }
    }
}