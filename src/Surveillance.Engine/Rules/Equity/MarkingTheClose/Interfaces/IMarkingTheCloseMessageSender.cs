using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        Task Send(IMarkingTheCloseBreach breach);
    }
}