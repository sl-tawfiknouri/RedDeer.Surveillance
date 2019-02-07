using System;
using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.DataLayer.Processes
{
    /// <summary>
    /// Tracks side effects
    /// </summary>
    public class SystemProcessOperationRuleRun : ISystemProcessOperationRuleRun
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        public string CorrelationId { get; set; }

        public string SystemProcessId { get; set; }

        /// <summary>
        /// Foreign Key to Operation
        /// </summary>
        public int SystemProcessOperationId { get; set; }

        /// <summary>
        /// Link back to the rule enum
        /// </summary>
        public int RuleTypeId { get; set; }

        /// <summary>
        /// Record whether this is a back test
        /// </summary>
        public bool IsBackTest { get; set; }

        /// <summary>
        /// Rule run mode
        /// </summary>
        public bool IsForceRun { get; set; }

        /// <summary>
        /// The rule being executed
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>
        /// The version of the rule being executed
        /// </summary>
        public string RuleVersion { get; set; }

        /// <summary>
        /// The parameter id for the rule
        /// </summary>
        public string RuleParameterId { get; set; }

        /// <summary>
        /// The starting point for the rule run data
        /// </summary>
        public DateTime? ScheduleRuleStart { get; set; }

        /// <summary>
        /// The end point for the rule run data
        /// </summary>
        public DateTime? ScheduleRuleEnd { get; set; }

        public override string ToString()
        {
            return $"SystemProcessOperationRuleRun | Id {Id} | CorrelationId {CorrelationId} | SystemProcessId {SystemProcessId} | SystemProcessOperationId {SystemProcessOperationId} | RuleDescription {RuleDescription} | RuleVersion {RuleVersion} | ScheduleRuleStart {ScheduleRuleStart} | ScheduleRuleEnd {ScheduleRuleEnd} | RuleParameterId {RuleParameterId} | RuleTypeId {RuleTypeId} | IsBackTest {IsBackTest}";
        }
    }
}