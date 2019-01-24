using System;

namespace DomainV2.Trading
{
    public class RuleBreach
    {
        public RuleBreach(
            long id,
            string ruleId,
            string correlationId,
            bool isBackTest,
            DateTime createdOn,
            string title,
            string description,
            string venue,
            DateTime startOfPeriodUnderInvestigation,
            DateTime endOfPeriodUnderInvestigation,
            string assetCfi,
            string reddeerEnrichmentId,
            string systemOperationId)
        {
            Id = id;
            RuleId = ruleId;
            CorrelationId = correlationId;
            IsBackTest = isBackTest;
            CreatedOn = createdOn;
            Title = title;
            Description = description;
            Venue = venue;
            StartOfPeriodUnderInvestigation = startOfPeriodUnderInvestigation;
            EndOfPeriodUnderInvestigation = endOfPeriodUnderInvestigation;
            AssetCfi = assetCfi;
            ReddeerEnrichmentId = reddeerEnrichmentId;
            SystemOperationId = systemOperationId;
        }

        public long Id { get; set; }
        public string RuleId { get; set; }
        public string CorrelationId { get; set; }
        public bool IsBackTest { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Venue { get; set; }
        public DateTime StartOfPeriodUnderInvestigation { get; set; }
        public DateTime EndOfPeriodUnderInvestigation { get; set; }
        public string AssetCfi { get; set; }
        public string ReddeerEnrichmentId { get; set; }
        public string SystemOperationId { get; set; }
    }
}
