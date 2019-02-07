namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsConfiguration
    {
        string DataSynchroniserRequestQueueName { get; }
        string ScheduledRuleQueueName { get; }
        string CaseMessageQueueName { get; }
        string ScheduleRuleDistributedWorkQueueName { get; }
        string TestRuleRunUpdateQueueName { get; }
        string AuroraConnectionString { get; }
    }
}