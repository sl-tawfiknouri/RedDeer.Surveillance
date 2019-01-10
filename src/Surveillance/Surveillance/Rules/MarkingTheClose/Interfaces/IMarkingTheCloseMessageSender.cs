using System.Threading.Tasks;

namespace Surveillance.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        Task Send(IMarkingTheCloseBreach breach);
    }
}