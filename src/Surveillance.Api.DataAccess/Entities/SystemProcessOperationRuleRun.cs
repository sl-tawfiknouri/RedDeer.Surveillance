using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcessOperationRuleRun : ISystemProcessOperationRuleRun
    {
        public SystemProcessOperationRuleRun()
        {
        }

        [Key]
        public int Id { get; set; }
        public int SystemProcessOperationId { get; set; }
        public string RuleDescription { get; set; }
        public string RuleVersion { get; set; }
        public DateTime ScheduleRuleStart { get; set; }
        public DateTime ScheduleRuleEnd { get; set; }
        public string CorrelationId { get; set; }
        public string RuleParameterId { get; set; }
        public int RuleTypeId { get; set; }
        public bool IsBackTest { get; set; }
        public bool IsForceRun { get; set; }
    }
}
