using System;

namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }
        string SystemProcessId { get; set; }
        int SystemProcessOperationId { get; set; }
        DateTime? ScheduleRuleInitialStart { get; set; }
        DateTime? ScheduleRuleInitialEnd { get; set; }
        string RulesDistributed { get; set; }
    }
}