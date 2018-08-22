namespace TestHarness.Commands
{
    public interface ICommand
    {
        void Run();

        bool Handles(string command);
    }
}