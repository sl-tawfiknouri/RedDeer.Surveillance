using System;
using Surveillance.System.DataLayer.Processes.Interfaces;

namespace Surveillance.System.DataLayer.Processes
{
    public class SystemProcessOperationDistributeRule : ISystemProcessOperationDistributeRule
    {
        public int Id { get; set; }

        public int SystemProcessOperationId { get; set; }

        public DateTime? InitialStart { get; set; }

        public DateTime? InitialEnd { get; set; }

        public string RulesDistributed { get; set; }
    }
}
