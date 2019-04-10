namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface ISystemProcessOperationDataSynchroniser
    {
        int Id { get; set; }
        string QueueMessageId { get; set; }
        int RuleRunId { get; }
        int SystemProcessOperationId { get; set; }
    }
}