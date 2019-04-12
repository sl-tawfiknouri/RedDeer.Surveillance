using System;
using System.Globalization;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcessOperationDistributeRule : ISystemProcessOperationDistributeRule
    {
        public int Id { get; set; }
        public int SystemProcessOperationId { get; set; }
        public DateTime ScheduleRuleInitialStart { get; set; }
        public DateTime? ScheduleRuleInitialEnd { get; set; }
        public string RulesDistributed { get; set; }

        public string InitialStart => ScheduleRuleInitialStart.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
        public string InitialEnd => ScheduleRuleInitialEnd?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;
    }
}
