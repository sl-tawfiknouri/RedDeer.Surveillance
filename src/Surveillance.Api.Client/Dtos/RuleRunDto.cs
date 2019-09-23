namespace RedDeer.Surveillance.Api.Client.Dtos
{
    using System;

    public class RuleRunDto
    {
        public string CorrelationId { get; set; }

        public int Id { get; set; }

        public ProcessOperationDto ProcessOperation { get; set; }

        public string RuleDescription { get; set; }

        public string RuleVersion { get; set; }

        public DateTime ScheduleRuleEnd { get; set; }

        public DateTime ScheduleRuleStart { get; set; }
    }
}