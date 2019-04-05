namespace Surveillance.Api.App.Authorization
{
    public static class PolicyManifest
    {
        public const string AdminPolicy = "AdminPolicy";
        public const string AdminReaderPolicy = "AdminReaderPolicy";
        public const string AdminWriterPolicy = "AdminWriterPolicy";

        public static string UserPolicy = "UserPolicy";
        public static string UserReaderPolicy = "UserReaderPolicy";
        public static string UserWriterPolicy = "UserWriterPolicy";

        public static string ClaimName = "Reddeer";
    }
}
