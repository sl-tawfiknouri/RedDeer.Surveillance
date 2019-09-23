namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    using System;

    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }

        string RulesDistributed { get; set; }

        DateTime? ScheduleRuleInitialEnd { get; set; }

        DateTime? ScheduleRuleInitialStart { get; set; }

        string SystemProcessId { get; set; }

        int SystemProcessOperationId { get; set; }
    }
}