﻿using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
{
    /// <summary>
    /// Tracks side effects
    /// </summary>
    public class SystemProcessOperationRuleRun : ISystemProcessOperationRuleRun
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Foreign Key to Operation
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// The rule being executed
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>
        /// The version of the rule being executed
        /// </summary>
        public string RuleVersion { get; set; }

        /// <summary>
        /// The starting point for the rule run data
        /// </summary>
        public DateTime? RuleScheduleBegin { get; set; }

        /// <summary>
        /// The end point for the rule run data
        /// </summary>
        public DateTime? RuleScheduleEnd { get; set; }
    }
}