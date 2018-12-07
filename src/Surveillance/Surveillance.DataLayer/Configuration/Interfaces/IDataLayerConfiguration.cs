using Utilities.Aws_IO.Interfaces;

namespace Surveillance.DataLayer.Configuration.Interfaces
{
    public interface IDataLayerConfiguration : IAwsConfiguration
    {
        string SurveillanceUserApiAccessToken { get; set; }
        string ClientServiceUrl { get; set; }
    }
}