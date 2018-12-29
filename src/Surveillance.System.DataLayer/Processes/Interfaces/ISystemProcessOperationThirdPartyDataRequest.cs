﻿namespace Surveillance.System.DataLayer.Processes.Interfaces
{
    public interface ISystemProcessOperationThirdPartyDataRequest
    {
        int Id { get; set; }
        string QueueMessageId { get; set; }
        string RuleId { get; set; }
        int SystemProcessOperationId { get; set; }
        string SystemProcessId { get; set; }
    }
}