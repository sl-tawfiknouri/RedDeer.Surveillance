using System.Threading.Tasks;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        Task Send(ICancelledOrderRuleBreach ruleBreach);
    }
}