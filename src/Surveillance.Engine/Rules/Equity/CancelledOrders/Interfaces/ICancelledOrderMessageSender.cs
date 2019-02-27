using System.Threading.Tasks;

namespace Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        Task Send(ICancelledOrderRuleBreach ruleBreach);
    }
}