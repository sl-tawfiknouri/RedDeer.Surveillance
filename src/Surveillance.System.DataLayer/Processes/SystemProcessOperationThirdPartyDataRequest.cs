namespace Surveillance.Auditing.DataLayer.Processes
{
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public class SystemProcessOperationThirdPartyDataRequest : ISystemProcessOperationThirdPartyDataRequest
    {
        public int Id { get; set; }

        public string QueueMessageId { get; set; }

        /// <summary>
        ///     If requested via a rule - what was its id
        /// </summary>
        public string RuleRunId { get; set; }

        public string SystemProcessId { get; set; }

        public int SystemProcessOperationId { get; set; }

        public override string ToString()
        {
            return
                $"Id {this.Id} | SystemProcessOperationId {this.SystemProcessOperationId} | QueueMessageId {this.QueueMessageId} | RuleId {this.RuleRunId}";
        }
    }
}