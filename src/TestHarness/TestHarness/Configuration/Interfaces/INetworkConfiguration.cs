namespace TestHarness.Configuration.Interfaces
{
    using Infrastructure.Network.Aws.Interfaces;

    public interface INetworkConfiguration : IAwsConfiguration
    {
        string ClientServiceUrl { get; set; }

        string SurveillanceUserApiAccessToken { get; set; }
    }
}