
// ReSharper disable UnusedMember.Global
namespace TestHarness.Commands
{
    using TestHarness.Commands.Interfaces;

    public class CommandManifest : ICommandManifest
    {
        public string Help => "help";

        public string Initiate => "initiate";

        public string Quit => "quit";

        public string RunCancelledTrade => "run cancelled trade";

        public string RunDemo => "run demo";

        public string RunDemoCsv => "run demo csv";

        public string RunDemoWithNetworking => "run demo networking";

        public string RunSpoofedTrade => "run spoofed trade";

        public string StopDemo => "stop demo";

        public string StopDemoCsv => "stop demo csv";

        public string StopDemoWithNetworking => "stop demo networking";
    }
}