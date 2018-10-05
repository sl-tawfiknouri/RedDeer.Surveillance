using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }
        int SystemProcessOperationId { get; set; }
        DateTime? InitialEnd { get; set; }
        DateTime? InitialStart { get; set; }
        string RulesDistributed { get; set; }
    }
}