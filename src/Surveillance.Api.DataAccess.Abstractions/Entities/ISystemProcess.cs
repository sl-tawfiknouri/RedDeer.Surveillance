using System;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcess
    {
        string Id { get; set; }
        string MachineId { get; set; }
        string ProcessId { get; set; }
        int SystemProcessTypeId { get; set; }
        DateTime? InstanceInitiated { get; set; }
        DateTime? Heartbeat { get; set; }
    }
}