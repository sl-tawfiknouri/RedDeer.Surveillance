// ReSharper disable UnusedMember.Global
namespace TestHarness.Interfaces
{
    public interface IMediator
    {
        void Initiate();
        void Terminate();
        void ActionCommand(string command);
    }
}