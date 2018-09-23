namespace Surveillance.Rules.Marking_The_Close.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        void Send(IMarkingTheCloseBreach breach);
    }
}