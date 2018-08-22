namespace TestHarness.Commands.Interfaces
{
    public interface ICommand
    {
        void Run(string command);

        bool Handles(string command);
    }
}