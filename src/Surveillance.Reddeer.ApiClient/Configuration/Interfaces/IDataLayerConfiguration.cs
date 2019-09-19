namespace Surveillance.Reddeer.ApiClient.Configuration.Interfaces
{
    using Infrastructure.Network.Aws.Interfaces;

    /// <summary>
    /// The Client Configuration interface.
    /// </summary>
    public interface IApiClientConfiguration : IAwsConfiguration
    {
        /// <summary>
        /// Gets or sets the service url.
        /// </summary>
        string BmllServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the client service url.
        /// </summary>
        string ClientServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets the surveillance user access token.
        /// </summary>
        string SurveillanceUserApiAccessToken { get; set; }
    }
}