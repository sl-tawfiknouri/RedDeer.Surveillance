// ReSharper disable UnusedMember.Global

namespace TestHarness.Interfaces
{
    public interface IMediator
    {
        void ActionCommand(string command);

        void Initiate();

        void Terminate();
    }
}