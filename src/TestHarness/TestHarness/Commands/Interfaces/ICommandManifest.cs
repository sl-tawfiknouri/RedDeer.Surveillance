// ReSharper disable UnusedMember.Global

namespace TestHarness.Commands.Interfaces
{
    public interface ICommandManifest
    {
        string Help { get; }

        string Initiate { get; }

        string Quit { get; }

        string RunDemo { get; }

        string RunDemoWithNetworking { get; }

        string RunSpoofedTrade { get; }

        string StopDemo { get; }

        string StopDemoWithNetworking { get; }
    }
}