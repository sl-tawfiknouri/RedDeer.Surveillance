namespace Infrastructure.Network.Aws.Interfaces
{
    public interface IAwsConfiguration
    {
        string AuroraConnectionString { get; }

        string CaseMessageQueueName { get; }

        string DataSynchroniserRequestQueueName { get; }

        string EmailServiceSendEmailQueueName { get; }

        string ScheduleDelayedRuleRunQueueName { get; }

        string ScheduledRuleQueueName { get; }

        string ScheduleRuleCancellationQueueName { get; }

        string ScheduleRuleDistributedWorkQueueName { get; }

        string TestRuleRunUpdateQueueName { get; }

        string UploadCoordinatorQueueName { get; }
    }
}