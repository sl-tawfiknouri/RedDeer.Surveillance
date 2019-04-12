using System.ComponentModel.DataAnnotations;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class SystemProcessOperationDataSynchroniser : ISystemProcessOperationDataSynchroniser
    {
        public SystemProcessOperationDataSynchroniser()
        {
        }

        [Key]
        public int Id { get; set; }
        public int SystemProcessOperationId { get; set; }
        public string QueueMessageId { get; set; }
        public string RuleRunId { get; set; }

        int ISystemProcessOperationDataSynchroniser.RuleRunId
        {
            get => int.Parse(RuleRunId);
        }

    }
}
