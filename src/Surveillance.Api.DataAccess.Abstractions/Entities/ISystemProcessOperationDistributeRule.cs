using System;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }
        string RulesDistributed { get; set; }
        int SystemProcessOperationId { get; set; }

        DateTime ScheduleRuleInitialStart { get; set; }
        DateTime? ScheduleRuleInitialEnd { get; set; }
    }
}