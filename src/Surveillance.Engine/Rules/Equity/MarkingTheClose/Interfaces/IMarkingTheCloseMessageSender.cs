namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    using System.Threading.Tasks;

    public interface IMarkingTheCloseMessageSender
    {
        Task Send(IMarkingTheCloseBreach breach);
    }
}