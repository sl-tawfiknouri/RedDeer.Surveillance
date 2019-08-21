namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    public interface ISystemProcessOperation
    {
        int Id { get; set; }

        DateTime? OperationEnd { get; }

        DateTime OperationStart { get; }

        int OperationState { get; set; }

        string SystemProcessId { get; set; }
    }
}