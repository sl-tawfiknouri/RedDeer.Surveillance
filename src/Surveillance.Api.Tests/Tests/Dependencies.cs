using RedDeer.Surveillance.Api.Client;
using Surveillance.Api.Tests.Infrastructure;

namespace Surveillance.Api.Tests.Tests
{
    public static class Dependencies
    {
        public static DbContext DbContext { get; set; }
        public static ApiClient ApiClient { get; set; }
    }
}
