using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer.Configuration.Interfaces
{
    public interface IDataLayerConfiguration : IAwsConfiguration
    {
        string ElasticSearchDomain { get; set; }
        string ElasticSearchPort { get; set; }
        string ElasticSearchProtocol { get; set; }
        string SurveillanceUserApiAccessToken { get; set; }
        string ClientServiceUrl { get; set; }
    }
}