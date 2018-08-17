using TestHarness.Commands;

namespace TestHarness.Interfaces
{
    public interface IMediator
    {
        void ActionCommand();
        void Initiate(InitiateCommand command);
        void Terminate();
    }
}