using TestHarness.Commands.Interfaces;

namespace TestHarness.Commands
{
    public class CommandManifest : ICommandManifest
    {
        public string Initiate { get { return "initiate"; } }
        public string Quit { get { return "quit"; } }
        public string Help { get { return "help"; } }
    }
}
