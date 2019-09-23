namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    public interface ISystemProcess
    {
        DateTime? Heartbeat { get; set; }

        string Id { get; set; }

        DateTime? InstanceInitiated { get; set; }

        string MachineId { get; set; }

        string ProcessId { get; set; }

        int SystemProcessTypeId { get; set; }
    }
}