namespace Surveillance.Auditing.DataLayer.Processes.Interfaces
{
    using System;

    public interface ISystemProcessOperationRuleRun
    {
        string CorrelationId { get; set; }

        int Id { get; set; }

        bool IsBackTest { get; set; }

        bool IsForceRun { get; set; }

        string RuleDescription { get; set; }

        string RuleParameterId { get; set; }

        int RuleTypeId { get; set; }

        string RuleVersion { get; set; }

        DateTime? ScheduleRuleEnd { get; set; }

        DateTime? ScheduleRuleStart { get; set; }

        string SystemProcessId { get; set; }

        int SystemProcessOperationId { get; set; }
    }
}