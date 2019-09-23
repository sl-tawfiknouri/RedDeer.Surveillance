namespace Surveillance.Api.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessOperationRuleRun : ISystemProcessOperationRuleRun
    {
        public string CorrelationId { get; set; }

        [Key]
        public int Id { get; set; }

        public bool IsBackTest { get; set; }

        public bool IsForceRun { get; set; }

        public string RuleDescription { get; set; }

        public string RuleParameterId { get; set; }

        public int RuleTypeId { get; set; }

        public string RuleVersion { get; set; }

        public DateTime ScheduleRuleEnd { get; set; }

        public DateTime ScheduleRuleStart { get; set; }

        public int SystemProcessOperationId { get; set; }
    }
}