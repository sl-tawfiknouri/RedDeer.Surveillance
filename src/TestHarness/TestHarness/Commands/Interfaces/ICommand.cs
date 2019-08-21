namespace TestHarness.Commands.Interfaces
{
    public interface ICommand
    {
        bool Handles(string command);

        void Run(string command);
    }
}