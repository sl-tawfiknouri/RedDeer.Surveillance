namespace Surveillance.Api.DataAccess.Entities
{
    using System;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessOperationDistributeRule : ISystemProcessOperationDistributeRule
    {
        public int Id { get; set; }

        public string RulesDistributed { get; set; }

        public DateTime? ScheduleRuleInitialEnd { get; set; }

        public DateTime ScheduleRuleInitialStart { get; set; }

        public int SystemProcessOperationId { get; set; }
    }
}