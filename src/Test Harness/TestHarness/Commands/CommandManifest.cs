using TestHarness.Commands.Interfaces;

namespace TestHarness.Commands
{
    public class CommandManifest : ICommandManifest
    {
        public string Initiate { get { return "initiate"; } }
        public string Quit { get { return "quit"; } }
        public string Help { get { return "help"; } }
        public string RunDemo { get { return "run demo"; } }
        public string StopDemo { get { return "stop demo"; } }
        public string RunDemoWithNetworking { get { return "run demo networking"; } }
        public string StopDemoWithNetworking { get { return "stop demo networking"; } }
        public string BuyProhibitedSecurity { get { return "run prohibited trade"; } }
        public string RunSpoofedTrade { get { return "run spoofed trade"; } }
        public string RunCancelledTrade { get { return "run cancelled trade"; } }
    }
}
