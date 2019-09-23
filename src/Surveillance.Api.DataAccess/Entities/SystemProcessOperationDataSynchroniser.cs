namespace Surveillance.Api.DataAccess.Entities
{
    using System.ComponentModel.DataAnnotations;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessOperationDataSynchroniser : ISystemProcessOperationDataSynchroniser
    {
        [Key]
        public int Id { get; set; }

        public string QueueMessageId { get; set; }

        public string RuleRunId { get; set; }

        public int SystemProcessOperationId { get; set; }

        int ISystemProcessOperationDataSynchroniser.RuleRunId => int.Parse(this.RuleRunId);
    }
}