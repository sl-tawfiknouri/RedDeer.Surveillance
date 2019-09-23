namespace Surveillance.DataLayer.Configuration.Interfaces
{
    using Infrastructure.Network.Aws.Interfaces;

    public interface IDataLayerConfiguration : IAwsConfiguration
    {
        string BmllServiceUrl { get; set; }

        string ClientServiceUrl { get; set; }

        string SurveillanceUserApiAccessToken { get; set; }
    }
}