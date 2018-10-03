using System;

namespace Surveillance.System.Auditing.Processes.Interfaces
{
    public interface ISystemProcessOperationRuleRun
    {
        string Id { get; set; }
        string OperationId { get; set; }
        string RuleDescription { get; set; }
        DateTime? RuleScheduleBegin { get; set; }
        DateTime? RuleScheduleEnd { get; set; }
        string RuleVersion { get; set; }
    }
}