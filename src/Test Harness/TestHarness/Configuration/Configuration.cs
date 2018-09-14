using TestHarness.Configuration.Interfaces;

namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        /// <summary>
        /// Use defaults
        /// </summary>
        public Configuration()
        {
        }

        public string TradeWebsocketUriDomain { get; set; }
        public string TradeWebsocketUriPort { get; set; }

        public string StockExchangeDomainUriDomainSegment { get; set; }
        public string StockExchangeDomainUriPort { get; set; }
        public string ScheduledRuleQueueName {get; set;}
        public string CaseMessageQueueName { get; set; }

        public bool IsEc2Instance { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
    }
}
