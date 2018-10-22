namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        void Send(IMarkingTheCloseBreach breach);
    }
}