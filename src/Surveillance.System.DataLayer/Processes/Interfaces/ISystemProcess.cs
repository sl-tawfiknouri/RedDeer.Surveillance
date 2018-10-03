﻿using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcess
    {
        DateTime Heartbeat { get; set; }
        string InstanceId { get; set; }
        DateTime InstanceInitiated { get; set; }
        string MachineId { get; set; }
        string ProcessId { get; set; }
    }
}