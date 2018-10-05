using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperation
    {
        int Id { get; set; }
        string SystemProcessId { get; set; }
        DateTime OperationStart { get; set; }
        DateTime? OperationEnd { get; set; }
        OperationState OperationState { get; set; }
    }
}