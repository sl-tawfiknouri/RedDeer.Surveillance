using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }
        int SystemProcessOperationId { get; set; }
        DateTime? ScheduleRuleInitialStart { get; set; }
        DateTime? ScheduleRuleInitialEnd { get; set; }
        string RulesDistributed { get; set; }
    }
}