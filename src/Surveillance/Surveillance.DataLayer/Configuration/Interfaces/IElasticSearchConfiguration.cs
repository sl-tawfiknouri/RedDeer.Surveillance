using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer.Configuration.Interfaces
{
    public interface IElasticSearchConfiguration : IAwsConfiguration
    {
        string ElasticSearchDomain { get; set; }
        string ElasticSearchPort { get; set; }
        string ElasticSearchProtocol { get; set; }
    }
}