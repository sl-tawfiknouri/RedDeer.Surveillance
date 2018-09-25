namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsConfiguration
    {
        bool IsEc2Instance { get; }
        string ScheduledRuleQueueName { get; }
        string CaseMessageQueueName { get; }
    }
}