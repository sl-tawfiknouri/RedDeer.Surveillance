using Surveillance.DataLayer.Configuration.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer.Configuration
{
    public class ElasticSearchConfiguration : IElasticSearchConfiguration
    {
        public bool IsEc2Instance { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string ElasticSearchDomain { get; set; }
        public string ElasticSearchPort { get; set; }
        public string ElasticSearchProtocol { get; set; }
    }
}
