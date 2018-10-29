namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsConfiguration
    {
        string ScheduledRuleQueueName { get; }
        string CaseMessageQueueName { get; }
        string ScheduleRuleDistributedWorkQueueName { get; }

        string ScheduledRuleDeadLetterQueueName { get; }
        string CaseMessageDeadLetterQueueName { get; }
        string ScheduleRuleDistributedWorkDeadLetterQueueName { get; }

        string AuroraConnectionString { get; }
    }
}