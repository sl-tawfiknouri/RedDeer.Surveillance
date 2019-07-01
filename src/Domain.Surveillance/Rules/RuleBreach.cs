using System;
using System.Collections.Generic;

namespace Domain.Surveillance.Rules
{
    public class RuleBreach
    {
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
            OrganisationalFactor = organisationalFactor;
            OrganisationalFactorValue = organisationalFactorValue ?? string.Empty;
            ParameterTuning = parameterTuning;
            RuleBreachOrderIds = ruleBreachOrderIds ?? new List<int>();
        }

        public int? Id { get; set; }
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
        public int OrganisationalFactor { get; set; }
        public string OrganisationalFactorValue { get; set; }
        public bool ParameterTuning { get; set; }

        public IReadOnlyCollection<int> RuleBreachOrderIds { get; set; }
    }
}
