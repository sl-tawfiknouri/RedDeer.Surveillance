using System;
using System.Collections.Generic;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class RuleBreachDto
    {
        public int Id { get; set; }
        public string RuleId { get; set; }
        public string CorrelationId { get; set; }
        public bool IsBackTest { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public string AssetCfi { get; set; }
        public string ReddeerEnrichmentId { get; set; }
        public int SystemOperationId { get; set; }
        public List<OrderDto> Orders { get; set; }

        public DateTime StartOfPeriodUnderInvestigation { get; set; }
        public DateTime EndOfPeriodUnderInvestigation { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
