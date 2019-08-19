namespace Surveillance.Reddeer.ApiClient.Configuration.Interfaces
{
    using Infrastructure.Network.Aws.Interfaces;

    public interface IApiClientConfiguration : IAwsConfiguration
    {
        string BmllServiceUrl { get; set; }

        string ClientServiceUrl { get; set; }

        string SurveillanceUserApiAccessToken { get; set; }
    }
}