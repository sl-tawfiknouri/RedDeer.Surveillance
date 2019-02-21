using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        Task Send(ICancelledOrderRuleBreach ruleBreach);
    }
}