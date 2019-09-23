namespace Domain.Surveillance.Rules
{
    using System;
    using System.Collections.Generic;

    public class RuleBreach
    {
        public RuleBreach()
        {
        }

        public RuleBreach(
            int? id,
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
            string systemOperationId,
            int organisationalFactor,
            string organisationalFactorValue,
            bool parameterTuning,
            IReadOnlyCollection<int> ruleBreachOrderIds)
        {
            this.Id = id;
            this.RuleId = ruleId;
            this.CorrelationId = correlationId;
            this.IsBackTest = isBackTest;
            this.CreatedOn = createdOn;
            this.Title = title;
            this.Description = description;
            this.Venue = venue;
            this.StartOfPeriodUnderInvestigation = startOfPeriodUnderInvestigation;
            this.EndOfPeriodUnderInvestigation = endOfPeriodUnderInvestigation;
            this.AssetCfi = assetCfi;
            this.ReddeerEnrichmentId = reddeerEnrichmentId;
            this.SystemOperationId = systemOperationId;
            this.OrganisationalFactor = organisationalFactor;
            this.OrganisationalFactorValue = organisationalFactorValue ?? string.Empty;
            this.ParameterTuning = parameterTuning;
            this.RuleBreachOrderIds = ruleBreachOrderIds ?? new List<int>();
        }

        public string AssetCfi { get; set; }

        public string CorrelationId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Description { get; set; }

        public DateTime EndOfPeriodUnderInvestigation { get; set; }

        public int? Id { get; set; }

        public bool IsBackTest { get; set; }

        public int OrganisationalFactor { get; set; }

        public string OrganisationalFactorValue { get; set; }

        public bool ParameterTuning { get; set; }

        public string ReddeerEnrichmentId { get; set; }

        public IReadOnlyCollection<int> RuleBreachOrderIds { get; set; }

        public string RuleId { get; set; }

        public DateTime StartOfPeriodUnderInvestigation { get; set; }

        public string SystemOperationId { get; set; }

        public string Title { get; set; }

        public string Venue { get; set; }
    }
}