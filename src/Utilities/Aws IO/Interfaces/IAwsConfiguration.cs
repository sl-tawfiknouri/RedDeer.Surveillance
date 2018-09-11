﻿namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsConfiguration
    {
        bool IsEc2Instance { get; }
        string AwsAccessKey { get; }
        string AwsSecretKey { get; }
        string ScheduledRuleQueueName { get; }
    }
}