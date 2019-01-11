using System;

namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationRuleRun
    {
        int Id { get; set; }
        string CorrelationId { get; set; }
        string SystemProcessId { get; set; }
        int SystemProcessOperationId { get; set; }
        string RuleDescription { get; set; }
        DateTime? ScheduleRuleStart { get; set; }
        DateTime? ScheduleRuleEnd { get; set; }
        string RuleVersion { get; set; }
        string RuleParameterId { get; set; }
        int RuleTypeId { get; set; }
        bool IsBackTest { get; set; }
        bool IsForceRun { get; set; }
    }
}