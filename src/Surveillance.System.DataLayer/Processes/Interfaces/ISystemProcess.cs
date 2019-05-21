using System;

namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    public interface ISystemProcess
    {
        DateTime Heartbeat { get; set; }
        string Id { get; set; }
        DateTime InstanceInitiated { get; set; }
        string MachineId { get; set; }
        string ProcessId { get; set; }
        SystemProcessType SystemProcessType { get; set; }
        bool CancelRuleQueueDeletedFlag { get; set; }
    }
}