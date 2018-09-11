namespace Surveillance.DataLayer.Configuration.Interfaces
{
    public interface IDataLayerConfiguration
    {
        bool IsEc2Instance { get; }
        string AwsAccessKey { get; }
        string AwsSecretKey { get; }
        string ScheduledRuleQueueName { get; }
        string ElasticSearchDomain { get; set; }
        string ElasticSearchPort { get; set; }
        string ElasticSearchProtocol { get; set; }
    }
}