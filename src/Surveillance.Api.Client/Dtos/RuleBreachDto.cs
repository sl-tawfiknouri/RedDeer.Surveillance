using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Surveillance.Api.Client.Dtos
{
    public class RuleBreachDto
    {
        public int Id { get; set; }
        public string RuleId { get; set; }
        public string CorrelationId { get; set; }
        public bool IsBackTest { get; set; }
        public string Created { get; set; }
        public string StartOfRuleBreachPeriod { get; set; }
        public string EndOfRuleBreachPeriod { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public string AssetCfi { get; set; }
        public string ReddeerEnrichmentId { get; set; }
        public int SystemOperationId { get; set; }
        public List<OrderDto> Orders { get; set; }

        public DateTime StartOfPeriodUnderInvestigation => DateTime.Parse(StartOfRuleBreachPeriod, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime EndOfPeriodUnderInvestigation => DateTime.Parse(EndOfRuleBreachPeriod, CultureInfo.GetCultureInfo("en-GB"));
        public DateTime CreatedOn => DateTime.Parse(Created, CultureInfo.GetCultureInfo("en-GB"));
    }
}
