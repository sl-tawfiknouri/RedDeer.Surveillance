using TestHarness.Commands.Interfaces;
// ReSharper disable UnusedMember.Global

namespace TestHarness.Commands
{
    public class CommandManifest : ICommandManifest
    {
        public string Initiate => "initiate";
        public string Quit => "quit";
        public string Help => "help";
        public string RunDemo => "run demo";
        public string StopDemo => "stop demo";
        public string RunDemoWithNetworking => "run demo networking";
        public string StopDemoWithNetworking => "stop demo networking";
        public string BuyProhibitedSecurity => "run prohibited trade";
        public string RunSpoofedTrade => "run spoofed trade";
        public string RunCancelledTrade => "run cancelled trade";
    }
}
