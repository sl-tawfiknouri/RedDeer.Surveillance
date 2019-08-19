namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    public interface ISystemProcessOperationDistributeRule
    {
        int Id { get; set; }

        string RulesDistributed { get; set; }

        DateTime? ScheduleRuleInitialEnd { get; set; }

        DateTime ScheduleRuleInitialStart { get; set; }

        int SystemProcessOperationId { get; set; }
    }
}