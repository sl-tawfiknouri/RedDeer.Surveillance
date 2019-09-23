namespace Surveillance.Api.Tests.Tests
{
    using RedDeer.Surveillance.Api.Client;

    using Surveillance.Api.Tests.Infrastructure;

    public static class Dependencies
    {
        public static ApiClient ApiClient { get; set; }

        public static DbContext DbContext { get; set; }
    }
}