using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
{
    public class SystemProcessOperationDistributeRule : ISystemProcessOperationDistributeRule
    {
        public int Id { get; set; }

        public string SystemProcessId { get; set; }

        public int SystemProcessOperationId { get; set; }

        public DateTime? ScheduleRuleInitialStart { get; set; }

        public DateTime? ScheduleRuleInitialEnd { get; set; }

        public string RulesDistributed { get; set; }
    }
}
