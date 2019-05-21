using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class RuleRunDto
    {
        public int Id { get; set; }
        public string CorrelationId { get; set; }
        public string RuleDescription { get; set; }
        public string RuleVersion { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public ProcessOperationDto ProcessOperation { get; set; }

        public DateTime ScheduleRuleStart => DateTime.Parse(Start, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime ScheduleRuleEnd => DateTime.Parse(End, CultureInfo.GetCultureInfo("en-GB"));
    }
}
