namespace Infrastructure.Network.Aws.Interfaces
{
    public interface IAwsConfiguration
    {
        string DataSynchroniserRequestQueueName { get; }
        string ScheduledRuleQueueName { get; }
        string CaseMessageQueueName { get; }
        string ScheduleRuleDistributedWorkQueueName { get; }
        string UploadCoordinatorQueueName { get; }
        string TestRuleRunUpdateQueueName { get; }
        string AuroraConnectionString { get; }
    }
}