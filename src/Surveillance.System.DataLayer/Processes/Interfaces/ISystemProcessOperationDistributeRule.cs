using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationDistributeRule
    {
        string Id { get; set; }
        DateTime InitialEnd { get; set; }
        DateTime InitialStart { get; set; }
        string OperationId { get; set; }
        string RulesDistributed { get; set; }
    }
}