namespace Surveillance.Auditing.DataLayer.Processes
{
    using System;

    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public class SystemProcessOperationDistributeRule : ISystemProcessOperationDistributeRule
    {
        public int Id { get; set; }

        public string RulesDistributed { get; set; }

        public DateTime? ScheduleRuleInitialEnd { get; set; }

        public DateTime? ScheduleRuleInitialStart { get; set; }

        public string SystemProcessId { get; set; }

        public int SystemProcessOperationId { get; set; }

        public override string ToString()
        {
            return
                $"SystemProcessOperationDistributeRule | Id {this.Id} | SystemProcessId {this.SystemProcessId} | SystemProcessOperationId {this.SystemProcessOperationId} | Initial Start {this.ScheduleRuleInitialStart} | Initial End {this.ScheduleRuleInitialEnd} | RulesDistributed {this.RulesDistributed}";
        }
    }
}