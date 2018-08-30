using System;
using Nest;

namespace Surveillance.ElasticSearchDtos
{
    /// <summary>
    /// A data type for representing a breach of MAR legalisation
    /// </summary>
    [ElasticsearchType(Name = "rule-breach-document")]
    public class RuleBreachDocument
    {
        /// <summary>
        /// A GUID for identifying the breach {Guid.TimeStamp}
        /// </summary>
        [Keyword]
        public string Id { get; set; }

        /// <summary>
        /// The type of the rule breach i.e. spoofing, layering...
        /// </summary>
        [Keyword]
        public int CategoryId { get; set; }

        /// <summary>
        /// Textual description of the category id underlying category
        /// </summary>
        [Text]
        public string CategoryDescription { get; set; }

        /// <summary>
        /// A high level description of the rule breach intended for the end client
        /// </summary>
        [Text]
        public string RuleBreachDescription { get; set; }

        /// <summary>
        /// The date we identified a breach and created a record
        /// </summary>
        [Date]
        public DateTime BreachRaisedOn { get; set; }

        /// <summary>
        /// Our best estimate of when the breach began
        /// </summary>
        [Date]
        public DateTime BreachCommencedOn { get; set; }

        /// <summary>
        /// The end calendar point for the rule breach.
        /// Nullable to indicate on-going breaches
        /// </summary>
        [Date]
        public DateTime? BreachTerminatedOn { get; set; }
    }
}