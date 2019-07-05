using Infrastructure.Network.Aws.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Configuration.Interfaces
{
    public interface IApiClientConfiguration : IAwsConfiguration
    {
        string SurveillanceUserApiAccessToken { get; set; }
        string ClientServiceUrl { get; set; }
        string BmllServiceUrl { get; set; }
    }
}