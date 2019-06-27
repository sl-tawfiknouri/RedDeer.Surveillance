using System;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class RuleRunDto
    {
        public int Id { get; set; }
        public string CorrelationId { get; set; }
        public string RuleDescription { get; set; }
        public string RuleVersion { get; set; }
        public ProcessOperationDto ProcessOperation { get; set; }

        public DateTime ScheduleRuleStart { get; set; }
        public DateTime ScheduleRuleEnd { get; set; }
    }
}
