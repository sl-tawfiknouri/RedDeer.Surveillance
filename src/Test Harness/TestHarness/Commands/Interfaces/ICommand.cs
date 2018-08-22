namespace TestHarness.Commands.Interfaces
{
    public interface ICommand
    {
        void Run();

        bool Handles(string command);
    }
}