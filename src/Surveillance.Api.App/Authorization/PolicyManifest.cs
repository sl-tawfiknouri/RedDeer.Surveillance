namespace Surveillance.Api.App.Authorization
{
    public static class PolicyManifest
    {
        public const string AdminPolicy = "AdminPolicy";
        public const string AdminReaderPolicy = "AdminReaderPolicy";
        public const string AdminWriterPolicy = "AdminWriterPolicy";

        public const string UserPolicy = "UserPolicy";
        public const string UserReaderPolicy = "UserReaderPolicy";
        public const string UserWriterPolicy = "UserWriterPolicy";

        public const string ClaimName = "Reddeer";
    }
}
