namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsConfiguration
    {
        string ScheduledRuleQueueName { get; }
        string CaseMessageQueueName { get; }
        string ScheduleRuleDistributedWorkQueueName { get; }
    }
}