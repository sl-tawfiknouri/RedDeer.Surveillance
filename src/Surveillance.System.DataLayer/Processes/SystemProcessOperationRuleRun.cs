namespace Surveillance.Auditing.DataLayer.Processes
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    /// <summary>
    ///     Tracks side effects
    /// </summary>
    public class SystemProcessOperationRuleRun : ISystemProcessOperationRuleRun
    {
        public string CorrelationId { get; set; }

        /// <summary>
        ///     Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Record whether this is a back test
        /// </summary>
        public bool IsBackTest { get; set; }

        /// <summary>
        ///     Rule run mode
        /// </summary>
        public bool IsForceRun { get; set; }

        /// <summary>
        ///     The rule being executed
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>
        ///     The parameter id for the rule
        /// </summary>
        public string RuleParameterId { get; set; }

        /// <summary>
        ///     Link back to the rule enum
        /// </summary>
        public int RuleTypeId { get; set; }

        /// <summary>
        ///     The version of the rule being executed
        /// </summary>
        public string RuleVersion { get; set; }

        /// <summary>
        ///     The end point for the rule run data
        /// </summary>
        public DateTime? ScheduleRuleEnd { get; set; }

        /// <summary>
        ///     The starting point for the rule run data
        /// </summary>
        public DateTime? ScheduleRuleStart { get; set; }

        public string SystemProcessId { get; set; }

        /// <summary>
        ///     Foreign Key to Operation
        /// </summary>
        public int SystemProcessOperationId { get; set; }

        public override string ToString()
        {
            return
                $"SystemProcessOperationRuleRun | Id {this.Id} | CorrelationId {this.CorrelationId} | SystemProcessId {this.SystemProcessId} | SystemProcessOperationId {this.SystemProcessOperationId} | RuleDescription {this.RuleDescription} | RuleVersion {this.RuleVersion} | ScheduleRuleStart {this.ScheduleRuleStart} | ScheduleRuleEnd {this.ScheduleRuleEnd} | RuleParameterId {this.RuleParameterId} | RuleTypeId {this.RuleTypeId} | IsBackTest {this.IsBackTest}";
        }
    }
}