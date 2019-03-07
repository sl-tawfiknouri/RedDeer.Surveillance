using Infrastructure.Network.Aws.Interfaces;

namespace TestHarness.Configuration.Interfaces
{
    public interface INetworkConfiguration : IAwsConfiguration
    {
        string ClientServiceUrl { get; set; }
        string SurveillanceUserApiAccessToken { get; set; }
    }
}
