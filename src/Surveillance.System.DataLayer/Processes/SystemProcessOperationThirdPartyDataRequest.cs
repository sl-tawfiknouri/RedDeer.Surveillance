using Surveillance.Auditing.DataLayer.Processes.Interfaces;

namespace Surveillance.Auditing.DataLayer.Processes
{
    public class SystemProcessOperationThirdPartyDataRequest : ISystemProcessOperationThirdPartyDataRequest
    {
        public int Id { get; set; }
        public int SystemProcessOperationId { get; set; }
        public string SystemProcessId { get; set; }
        public string QueueMessageId { get; set; }

        /// <summary>
        /// If requested via a rule - what was its id
        /// </summary>
        public string RuleRunId { get; set; }

        public override string ToString()
        {
            return $"Id {Id} | SystemProcessOperationId {SystemProcessOperationId} | QueueMessageId {QueueMessageId} | RuleId {RuleRunId}";
        }
    }
}
