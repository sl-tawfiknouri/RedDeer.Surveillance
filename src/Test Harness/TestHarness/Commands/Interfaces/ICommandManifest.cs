namespace TestHarness.Commands.Interfaces
{
    public interface ICommandManifest
    {
        string Help { get; }
        string Initiate { get; }
        string Quit { get; }
    }
}