﻿using TestHarness.Configuration.Interfaces;

namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        public string TradeWebsocketUriDomain { get; set; }
        public string TradeWebsocketUriPort { get; set; }

        public string StockExchangeDomainUriDomainSegment { get; set; }
        public string StockExchangeDomainUriPort { get; set; }
        public string ScheduledRuleQueueName {get; set;}
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string ScheduledRuleDeadLetterQueueName { get; set; }
        public string CaseMessageDeadLetterQueueName { get; set; }
        public string ScheduleRuleDistributedWorkDeadLetterQueueName { get; set; }
        public string AuroraConnectionString { get; set; }

        public bool IsEc2Instance { get; set; }
    }
}
