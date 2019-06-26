using System;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperation
    {
        int Id { get; set; }
        int OperationState { get; set; }
        string SystemProcessId { get; set; }

        DateTime OperationStart { get; }
        DateTime? OperationEnd { get; }
    }
}