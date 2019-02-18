using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.MarkingTheClose.Interfaces
{
    public interface IMarkingTheCloseMessageSender
    {
        Task Send(IMarkingTheCloseBreach breach);
    }
}