using TestHarness.Commands;

namespace TestHarness.Interfaces
{
    public interface IMediator
    {
        void Initiate(InitiateCommand command);
        void Terminate();
        void ActionCommand(string command);
    }
}