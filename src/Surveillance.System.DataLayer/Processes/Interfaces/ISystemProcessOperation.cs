namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    using System;

    public interface ISystemProcessOperation
    {
        int Id { get; set; }

        DateTime? OperationEnd { get; set; }

        DateTime OperationStart { get; set; }

        OperationState OperationState { get; set; }

        string SystemProcessId { get; set; }
    }
}