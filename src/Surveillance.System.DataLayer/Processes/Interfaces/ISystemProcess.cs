namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    using System;

    public interface ISystemProcess
    {
        bool CancelRuleQueueDeletedFlag { get; set; }

        DateTime Heartbeat { get; set; }

        string Id { get; set; }

        DateTime InstanceInitiated { get; set; }

        string MachineId { get; set; }

        string ProcessId { get; set; }

        SystemProcessType SystemProcessType { get; set; }
    }
}