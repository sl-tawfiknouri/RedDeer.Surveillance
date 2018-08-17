using TestHarness.Display.Interfaces;

namespace TestHarness.Display
{
    public interface IConsole : IBaseDisplay
    {
        void Initiate();
        void Terminate();
    }
}